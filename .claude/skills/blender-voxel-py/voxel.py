# Voxel model builder, Python-API edition. Runs in headless Blender (the `bpy` PyPI module),
# never in the user's open window. Same shapes, same build(), same rules as blender-voxel.
#   uv run --project <skill dir> python my_model.py      # my_model.py starts with:
#   exec(open('<skill dir>/voxel.py').read())
#   new_scene()                 # empty scene: no cube, camera, light or world
#   V = {}                      # (x, y, z) -> material key; +z up, -y = front
#   box(V, x0, x1, y0, y1, z0, z1, 'skin')   # inclusive ranges
#   hollow / tile / blob / disc / mirror_x / carve / rescale   (identical to blender-voxel)
#   r = build('Gnome', V, {'skin': (0.95, 0.62, 0.45), ...}, size=VOXEL_SIZE)
#   save_blend('gnome.blend')   # the deliverable: open it in Blender
#   export_glb('Gnome', 'gnome.glb')            # only when asked
#   preview('Gnome', 'prev/gnome')              # front / 3-4 / close-up PNGs (throwaway scene)
#
# Style: detailed, realistic, hard-edged. Real-world proportions, lots of small parts,
#   NO rounded corners, NO smooth shading, NO bevel/subdivision. Everything is boxes.
#
# Scale: VOXEL_SIZE = 1/64 m (1.56 cm) for characters, furniture, large props;
#   FINE_SIZE = 1/128 m (7.8 mm) for hand-held props and small parts. Smallest feature
#   (key, port, button) >= 2 voxels. Change resolution with rescale(), not size=.
#
# Godot notes: Blender -Y front exports as glTF +Z (Godot front); 1 unit = 1 m; origin at feet;
#   materials='palette' = 1-2 surfaces; collider=True adds '<name>-convcolonly'.
import bpy, bmesh, math, os

VOXEL_SIZE = 1 / 64   # metres per voxel (64 per metre)
FINE_SIZE = 1 / 128   # hand-held props
MAX_FACES = 40000     # warn above this after meshing


# ---------------------------------------------------------------- shapes

def _span(c, r):
    return range(math.floor(c - r), math.ceil(c + r) + 1)


def box(V, x0, x1, y0, y1, z0, z1, m):
    for x in range(x0, x1 + 1):
        for y in range(y0, y1 + 1):
            for z in range(z0, z1 + 1):
                V[(x, y, z)] = m


def hollow(V, x0, x1, y0, y1, z0, z1, m, t=1):
    """Box shell, wall thickness t voxels (bezels, frames, trays, open crates)."""
    for x in range(x0, x1 + 1):
        for y in range(y0, y1 + 1):
            for z in range(z0, z1 + 1):
                if (min(x - x0, x1 - x, y - y0, y1 - y, z - z0, z1 - z) < t):
                    V[(x, y, z)] = m


def tile(V, x0, x1, y0, y1, z0, z1, m, nx=1, ny=1, nz=1, dx=0, dy=0, dz=0):
    """Repeat a box nx*ny*nz times, stepping (dx, dy, dz) voxels: keys, vents, windows, tiles."""
    for i in range(nx):
        for j in range(ny):
            for k in range(nz):
                box(V, x0 + i * dx, x1 + i * dx, y0 + j * dy, y1 + j * dy,
                    z0 + k * dz, z1 + k * dz, m)


def blob(V, cx, cy, cz, rx, ry, rz, m):
    """Stair-stepped ellipsoid. Organic parts only (heads, fruit); hard-surface = box()."""
    if min(rx, ry, rz) <= 0:
        raise ValueError('blob radii must be > 0')
    for x in _span(cx, rx):
        for y in _span(cy, ry):
            for z in _span(cz, rz):
                if ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 + ((z - cz) / rz) ** 2 <= 1:
                    V[(x, y, z)] = m


def disc(V, cx, cy, z, r, m):
    for x in _span(cx, r):
        for y in _span(cy, r):
            if (x - cx) ** 2 + (y - cy) ** 2 <= r * r:
                V[(x, y, z)] = m


def mirror_x(V):
    """Copy everything on the +x side onto -x (x=0 column is the centre line)."""
    for (x, y, z), m in list(V.items()):
        if x > 0:
            V[(-x, y, z)] = m


def rescale(V, f):
    """Resample V by factor f (nearest voxel) so the real-world shape is kept when
    the voxel size changes, e.g. size 0.025 -> 0.0625 is rescale(V, 0.4).
    Returns a new dict; the caller reassigns V. Thin parts (< 1/f voxels) can vanish."""
    if f <= 0:
        raise ValueError('scale factor must be > 0')
    xs, ys, zs = ([p[i] for p in V] for i in range(3))
    lo = (min(xs), min(ys), min(zs))
    hi = (max(xs), max(ys), max(zs))
    out = {}
    rng = [range(math.floor(lo[i] * f), math.ceil((hi[i] + 1) * f)) for i in range(3)]
    for x in rng[0]:
        for y in rng[1]:
            for z in rng[2]:
                m = V.get((math.floor((x + .5) / f), math.floor((y + .5) / f), math.floor((z + .5) / f)))
                if m is not None:
                    out[(x, y, z)] = m
    return out


def carve(V, keys):
    """Remove voxels. keys: any iterable of (x, y, z), e.g. a dict built with box()."""
    for k in keys:
        V.pop(k, None)


# ---------------------------------------------------------------- meshing

FACES = [((1, 0, 0), [(1, 0, 0), (1, 1, 0), (1, 1, 1), (1, 0, 1)]),
         ((-1, 0, 0), [(0, 0, 0), (0, 0, 1), (0, 1, 1), (0, 1, 0)]),
         ((0, 1, 0), [(0, 1, 0), (0, 1, 1), (1, 1, 1), (1, 1, 0)]),
         ((0, -1, 0), [(0, 0, 0), (1, 0, 0), (1, 0, 1), (0, 0, 1)]),
         ((0, 0, 1), [(0, 0, 1), (1, 0, 1), (1, 1, 1), (0, 1, 1)]),
         ((0, 0, -1), [(0, 0, 0), (0, 1, 0), (1, 1, 0), (1, 0, 0)])]


def _naive_quads(V):
    """One quad per exposed voxel face: (material_key, [4 lattice corners])."""
    for (x, y, z), m in V.items():
        for (dx, dy, dz), tmpl in FACES:
            if (x + dx, y + dy, z + dz) not in V:
                yield m, [(x + a, y + b, z + c) for a, b, c in tmpl]


def _greedy_quads(V):
    """Merge exposed faces into the largest same-material rectangles per slice.

    Same winding as FACES: each template corner's 0/1 on the two in-plane axes
    is scaled to the rectangle's extent, so normals stay outward.
    """
    for normal, tmpl in FACES:
        ax = next(i for i in range(3) if normal[i])
        u, v = [i for i in range(3) if i != ax]
        slices = {}
        for p, m in V.items():
            q = (p[0] + normal[0], p[1] + normal[1], p[2] + normal[2])
            if q not in V:
                slices.setdefault(p[ax], {})[(p[u], p[v])] = m

        for s, mask in slices.items():
            done = set()
            for a, b in sorted(mask):
                if (a, b) in done:
                    continue
                m = mask[(a, b)]

                def free(cell):
                    return cell not in done and mask.get(cell) == m

                h = 1
                while free((a, b + h)):
                    h += 1
                w = 1
                while all(free((a + w, b + k)) for k in range(h)):
                    w += 1
                for i in range(w):
                    for k in range(h):
                        done.add((a + i, b + k))

                corners = []
                for t in tmpl:
                    c = [0, 0, 0]
                    c[ax] = s + t[ax]
                    c[u] = a + t[u] * w
                    c[v] = b + t[v] * h
                    corners.append(tuple(c))
                yield m, corners


# ---------------------------------------------------------------- materials

def _to_srgb(c):
    c = min(max(c, 0.0), 1.0)
    return 12.92 * c if c <= 0.0031308 else 1.055 * c ** (1 / 2.4) - 0.055


def _principled(mat):
    mat.use_nodes = True
    return next(n for n in mat.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')


def _set_pbr(bsdf, metal):
    bsdf.inputs['Metallic'].default_value = 1.0 if metal else 0.0
    bsdf.inputs['Roughness'].default_value = 0.3 if metal else 0.7


def _flat_material(name, rgb, metal):
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    bsdf = _principled(mat)
    c = (*rgb, 1)
    bsdf.inputs['Base Color'].default_value = c
    _set_pbr(bsdf, metal)
    mat.diffuse_color = c  # solid-view colour
    return mat


def _palette_image(name, keys, colors):
    """1px-per-colour strip. Colours are given linear (like Base Color) and stored sRGB."""
    img = bpy.data.images.get(name)
    if img and tuple(img.size) != (len(keys), 1):
        bpy.data.images.remove(img)
        img = None
    if not img:
        img = bpy.data.images.new(name, len(keys), 1, alpha=False)
    px = []
    for k in keys:
        px += [_to_srgb(ch) for ch in colors[k]] + [1.0]
    img.pixels[:] = px
    try:
        img.pack()  # keep it inside the .blend and embeddable in the .glb
    except RuntimeError:
        pass
    return img


def _palette_material(name, img, metal):
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    bsdf = _principled(mat)
    nt = mat.node_tree
    tex = nt.nodes.get('Palette') or nt.nodes.new('ShaderNodeTexImage')
    tex.name = 'Palette'
    tex.image = img
    tex.interpolation = 'Closest'  # exported as a nearest-filter sampler
    tex.location = (bsdf.location.x - 350, bsdf.location.y)
    if not bsdf.inputs['Base Color'].is_linked:
        nt.links.new(tex.outputs['Color'], bsdf.inputs['Base Color'])
    _set_pbr(bsdf, metal)
    return mat


# ---------------------------------------------------------------- scene helpers

def _remove_object(ob):
    for child in list(ob.children):
        _remove_object(child)
    me = ob.data
    bpy.data.objects.remove(ob)
    if me and not me.users:
        bpy.data.meshes.remove(me)


def new_scene():
    """Wipe to an empty factory scene (no cube, camera, light, world). Call once at the top."""
    bpy.ops.wm.read_factory_settings(use_empty=True)


# ---------------------------------------------------------------- build

_SHADES = (-1.0, -0.35, 0.35, 1.0)


def _apply_grain(V, colors, metallic, grain):
    """Split every key into 4 brightness variants chosen by a hash of the voxel position."""
    used = set(V.values())
    V2 = {}
    for (x, y, z), k in V.items():
        h = ((x * 73856093) ^ (y * 19349663) ^ (z * 83492791)) >> 3
        V2[(x, y, z)] = f'{k}~{h & 3}'
    colors2, metal2 = {}, set()
    for k in colors:
        if k not in used:
            continue
        for i, sh in enumerate(_SHADES):
            f = 1 + grain * sh
            colors2[f'{k}~{i}'] = tuple(min(max(c * f, 0.0), 1.0) for c in colors[k])
            if k in metallic:
                metal2.add(f'{k}~{i}')
    return V2, colors2, tuple(metal2)


def build(name, V, colors, size=VOXEL_SIZE, metallic=(), mesh='greedy', materials='palette',
          collider=False, replace=True, grain=0.08):
    """One mesh object at the origin, centred on x/y, feet on z=0, only exposed faces.

    mesh:      'greedy' (merged rectangles, default), 'faces' (one quad per voxel face),
               or 'dissolve' (Blender's dissolve_limit, may give concave n-gons)
    materials: 'palette' (one palette texture; 1 material, +1 if anything is metallic)
               or 'separate' (one flat material per colour key)
    collider:  add a '<name>-convcolonly' bounding box for Godot's importer
    replace:   delete an existing object called `name` (and its children) first
    grain:     per-voxel brightness noise (0 = off, 0.08 = subtle texture, 0.15 = strong);
               each colour key gets 4 shade variants picked by a position hash, so surfaces
               look textured (fabric, skin, wood) while staying flat-shaded and palette-based
    """
    if not V:
        raise ValueError('V is empty')
    if grain:
        V, colors, metallic = _apply_grain(V, colors, metallic, grain)
    warnings = []
    if min(abs(size - VOXEL_SIZE), abs(size - FINE_SIZE)) > 1e-9:
        warnings.append(f'size {size} not {VOXEL_SIZE} or {FINE_SIZE}: rescale shape coords instead')
    if mesh not in ('greedy', 'faces', 'dissolve'):
        raise ValueError(f"mesh must be 'greedy', 'faces' or 'dissolve', not {mesh!r}")
    if materials not in ('palette', 'separate'):
        raise ValueError(f"materials must be 'palette' or 'separate', not {materials!r}")
    missing = set(V.values()) - set(colors)
    if missing:
        raise KeyError(f'no colour defined for: {sorted(missing)}')

    if replace and (old := bpy.data.objects.get(name)):
        _remove_object(old)

    keys = list(colors)
    color_index = {k: i for i, k in enumerate(keys)}
    palette = materials == 'palette'

    if palette:
        img = _palette_image(f'{name}_palette', keys, colors)
        mats = [_palette_material(f'{name}_palette', img, False)]
        if any(k in metallic for k in keys):
            mats.append(_palette_material(f'{name}_palette_metal', img, True))
        mat_index = {k: (1 if k in metallic else 0) for k in keys}
    else:
        mats = [_flat_material(f'{name}_{k}', colors[k], k in metallic) for k in keys]
        mat_index = color_index

    xs = [x for x, _, _ in V]
    ys = [y for _, y, _ in V]
    zs = [z for _, _, z in V]
    ox = (min(xs) + max(xs) + 1) / 2
    oy = (min(ys) + max(ys) + 1) / 2
    oz = min(zs)

    def world(p):
        return ((p[0] - ox) * size, (p[1] - oy) * size, (p[2] - oz) * size)

    bm = bmesh.new()
    uv = bm.loops.layers.uv.new('UVMap') if palette else None
    verts = {}

    def vert(p):
        v = verts.get(p)
        if v is None:
            v = verts[p] = bm.verts.new(world(p))
        return v

    naive_faces = sum(1 for _ in _naive_quads(V))
    quads = _greedy_quads(V) if mesh == 'greedy' else _naive_quads(V)
    for m, corners in quads:
        f = bm.faces.new([vert(c) for c in corners])
        f.material_index = mat_index[m]
        f.smooth = False
        if palette:
            uv_co = ((color_index[m] + 0.5) / len(keys), 0.5)
            for loop in f.loops:
                loop[uv].uv = uv_co

    if mesh == 'dissolve':
        delimit = {'MATERIAL', 'UV'} if palette else {'MATERIAL'}
        bmesh.ops.dissolve_limit(bm, angle_limit=0.001, verts=bm.verts[:],
                                 edges=bm.edges[:], delimit=delimit)

    me = bpy.data.meshes.new(name)
    bm.to_mesh(me)
    bm.free()
    for mat in mats:
        me.materials.append(mat)
    ob = bpy.data.objects.new(name, me)
    coll = bpy.context.collection
    coll.objects.link(ob)

    col_name = None
    if collider:
        lo = world((min(xs), min(ys), min(zs)))
        hi = world((max(xs) + 1, max(ys) + 1, max(zs) + 1))
        cbm = bmesh.new()
        bmesh.ops.create_cube(cbm, size=1.0)
        for vtx in cbm.verts:
            vtx.co = [(lo[i] + hi[i]) / 2 + vtx.co[i] * (hi[i] - lo[i]) for i in range(3)]
        cme = bpy.data.meshes.new(f'{name}-convcolonly')
        cbm.to_mesh(cme)
        cbm.free()
        col = bpy.data.objects.new(f'{name}-convcolonly', cme)
        col.parent = ob
        col.display_type = 'WIRE'
        col.hide_render = True
        coll.objects.link(col)
        col_name = col.name

    if len(me.polygons) > MAX_FACES:
        warnings.append(f'{len(me.polygons)} faces > {MAX_FACES}: merge noisy colours or drop sub-visible detail')
    return {'object': ob.name, 'voxels': len(V), 'faces_naive': naive_faces,
            'faces': len(me.polygons), 'materials': len(mats),
            'height_voxels': max(zs) - oz + 1, 'size': size,
            'height_m': round((max(zs) - oz + 1) * size, 2), 'warnings': warnings,
            'collider': col_name}


# ---------------------------------------------------------------- save / export / preview

def _abs(filepath):
    """'//x' = next to the open .blend; otherwise relative to the current directory."""
    path = os.path.abspath(bpy.path.abspath(filepath))
    os.makedirs(os.path.dirname(path), exist_ok=True)
    return path

def save_blend(filepath):
    """Save the scene as a .blend (the deliverable). Returns the absolute path."""
    path = _abs(filepath)
    if not path.lower().endswith('.blend'):
        path += '.blend'
    bpy.ops.wm.save_as_mainfile(filepath=path)
    return path


def export_glb(obj, filepath):
    """Export one built object (plus its collider child) as a .glb for Godot."""
    ob = bpy.data.objects[obj] if isinstance(obj, str) else obj
    path = _abs(filepath)
    if not path.lower().endswith('.glb'):
        path += '.glb'
    for o in bpy.context.view_layer.objects:
        o.select_set(False)
    for o in [ob, *ob.children_recursive]:
        o.hide_set(False)
        o.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.export_scene.gltf(filepath=path, export_format='GLB', use_selection=True,
                              export_apply=True, export_yup=True)
    return path


def preview(obj, prefix, res=(960, 720)):
    """Render front, 3/4 and close-up PNGs (<prefix>_front.png, _34.png, _close.png) in a
    throwaway scene so the model scene keeps no camera or light. Returns the paths.
    The close-up looks at the front-top of the object (keys, ports, bezel)."""
    from mathutils import Vector
    ob = bpy.data.objects[obj] if isinstance(obj, str) else obj
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    lo = Vector(map(min, zip(*corners)))
    hi = Vector(map(max, zip(*corners)))
    ctr, dim = (lo + hi) / 2, hi - lo
    r = max(dim)
    sc = bpy.data.scenes.new('_preview')
    sc.render.engine = 'BLENDER_WORKBENCH'
    sh = sc.display.shading
    sh.light, sh.color_type, sh.show_cavity = 'STUDIO', 'TEXTURE', True
    sc.view_settings.view_transform = 'Standard'
    sc.render.resolution_x, sc.render.resolution_y = res
    sc.render.image_settings.file_format = 'PNG'
    for o in [ob, *ob.children_recursive]:
        sc.collection.objects.link(o)
    cam = bpy.data.objects.new('_cam', bpy.data.cameras.new('_cam'))
    sc.collection.objects.link(cam)
    sc.camera = cam
    shots = {'front': (Vector((0, -2.2 * r, 0)), ctr, 1.1 * r),
             '34': (Vector((1.3 * r, -1.6 * r, 0.9 * r)), ctr, 1.2 * r),
             'close': (Vector((0.5 * r, -0.9 * r, 0.5 * r)),
                       Vector((ctr.x, lo.y, hi.z - dim.z * 0.25)), 0.45 * r)}
    paths = []
    for tag, (off, target, ortho) in shots.items():
        cam.data.type = 'ORTHO' if tag == 'front' else 'PERSP'
        cam.data.ortho_scale = ortho * 1.6
        cam.data.lens = 50 if tag == 'close' else 35
        cam.location = target + off if tag != 'front' else Vector((ctr.x, lo.y - 2 * r, ctr.z))
        cam.rotation_euler = (target - cam.location).to_track_quat('-Z', 'Y').to_euler()
        sc.render.filepath = path = _abs(f'{prefix}_{tag}.png')
        bpy.ops.render.render(write_still=True, scene=sc.name)
        paths.append(path)
    for o in [ob, *ob.children_recursive]:
        sc.collection.objects.unlink(o)
    bpy.data.scenes.remove(sc)
    bpy.data.objects.remove(cam)
    return paths
