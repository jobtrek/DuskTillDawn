"""Pure-numpy voxel shapes (no Blender). Same behaviour as the engine's, so the SDK validates what Blender builds.
Grid..carve are copied from engine_blender.py; shell/ellipse are SDK additions inlined into build.py."""
import math
import numpy as np


class Grid:
    """Dense, auto-growing voxel array. a[x - ox, y - oy, z - oz] = material id (0 = empty)."""

    def __init__(self):
        self.a = np.zeros((0, 0, 0), np.uint16)
        self.o = (0, 0, 0)
        self.keys = []          # id - 1 -> key
        self._id = {}

    def kid(self, key):
        i = self._id.get(key)
        if i is None:
            self.keys.append(key)
            i = self._id[key] = len(self.keys)
        return i

    def ensure(self, lo, hi):
        """Grow so that inclusive lattice range lo..hi fits."""
        if self.a.size == 0:
            self.o = tuple(lo)
            self.a = np.zeros(tuple(h - l + 1 for l, h in zip(lo, hi)), np.uint16)
            return
        cur_lo = self.o
        cur_hi = tuple(o + s - 1 for o, s in zip(self.o, self.a.shape))
        new_lo = tuple(min(a, b) for a, b in zip(lo, cur_lo))
        new_hi = tuple(max(a, b) for a, b in zip(hi, cur_hi))
        if new_lo == cur_lo and new_hi == cur_hi:
            return
        # grow with headroom so repeated small writes do not realloc every time
        pad = [4 if n != c else 0 for n, c in zip(new_lo, cur_lo)]
        pad_hi = [4 if n != c else 0 for n, c in zip(new_hi, cur_hi)]
        new_lo = tuple(n - p for n, p in zip(new_lo, pad))
        new_hi = tuple(n + p for n, p in zip(new_hi, pad_hi))
        b = np.zeros(tuple(h - l + 1 for l, h in zip(new_lo, new_hi)), np.uint16)
        s = tuple(slice(c - n, c - n + sz) for c, n, sz in zip(cur_lo, new_lo, self.a.shape))
        b[s] = self.a
        self.a, self.o = b, new_lo

    def view(self, x0, x1, y0, y1, z0, z1):
        self.ensure((x0, y0, z0), (x1, y1, z1))
        o = self.o
        return self.a[x0 - o[0]:x1 - o[0] + 1, y0 - o[1]:y1 - o[1] + 1, z0 - o[2]:z1 - o[2] + 1]

    def count(self):
        return int(np.count_nonzero(self.a))

    def bounds(self):
        nz = np.argwhere(self.a)
        lo, hi = nz.min(0), nz.max(0)
        return tuple(lo + self.o), tuple(hi + self.o)


# ---------------------------------------------------------------- shapes

def box(V, x0, x1, y0, y1, z0, z1, m):
    if x1 < x0 or y1 < y0 or z1 < z0:
        return
    V.view(x0, x1, y0, y1, z0, z1)[...] = V.kid(m)


def hollow(V, x0, x1, y0, y1, z0, z1, m, t=1):
    """Box shell, wall thickness t voxels (bezels, frames, trays, open crates)."""
    v = V.view(x0, x1, y0, y1, z0, z1)
    k = V.kid(m)
    ix = np.arange(x1 - x0 + 1)[:, None, None]
    iy = np.arange(y1 - y0 + 1)[None, :, None]
    iz = np.arange(z1 - z0 + 1)[None, None, :]
    d = np.minimum(np.minimum(np.minimum(ix, x1 - x0 - ix), np.minimum(iy, y1 - y0 - iy)),
                   np.minimum(iz, z1 - z0 - iz))
    v[d < t] = k


def tile(V, x0, x1, y0, y1, z0, z1, m, nx=1, ny=1, nz=1, dx=0, dy=0, dz=0):
    """Repeat a box nx*ny*nz times, stepping (dx, dy, dz) voxels: keys, vents, windows, tiles."""
    V.ensure((x0, y0, z0), (x1 + (nx - 1) * dx, y1 + (ny - 1) * dy, z1 + (nz - 1) * dz))
    for i in range(nx):
        for j in range(ny):
            for k in range(nz):
                box(V, x0 + i * dx, x1 + i * dx, y0 + j * dy, y1 + j * dy, z0 + k * dz, z1 + k * dz, m)


def _span(c, r):
    return math.floor(c - r), math.ceil(c + r)


def blob(V, cx, cy, cz, rx, ry, rz, m):
    """Stair-stepped ellipsoid. Organic parts only (heads, fruit); hard-surface = box()."""
    if min(rx, ry, rz) <= 0:
        raise ValueError('blob radii must be > 0')
    (x0, x1), (y0, y1), (z0, z1) = _span(cx, rx), _span(cy, ry), _span(cz, rz)
    v = V.view(x0, x1, y0, y1, z0, z1)
    X, Y, Z = np.ogrid[x0:x1 + 1, y0:y1 + 1, z0:z1 + 1]
    v[((X - cx) / rx) ** 2 + ((Y - cy) / ry) ** 2 + ((Z - cz) / rz) ** 2 <= 1] = V.kid(m)


def disc(V, cx, cy, z, r, m):
    (x0, x1), (y0, y1) = _span(cx, r), _span(cy, r)
    v = V.view(x0, x1, y0, y1, z, z)
    X, Y = np.ogrid[x0:x1 + 1, y0:y1 + 1]
    v[:, :, 0][(X - cx) ** 2 + (Y - cy) ** 2 <= r * r] = V.kid(m)


def carve(V, region):
    """Remove voxels. region: a Grid (nonzero cells are removed, same lattice coords)."""
    nz = np.argwhere(region.a)
    if not len(nz):
        return
    pts = nz + region.o - V.o
    ok = np.all((pts >= 0) & (pts < V.a.shape), axis=1)
    pts = pts[ok]
    V.a[pts[:, 0], pts[:, 1], pts[:, 2]] = 0


def shell(V, x0, x1, y0, y1, z0, z1, m, t=1, open=()):
    """Hollow box like hollow(), but faces named in `open` (top/bottom/front/back/left/right) have no wall."""
    if x1 < x0 or y1 < y0 or z1 < z0:
        return
    v = V.view(x0, x1, y0, y1, z0, z1)
    ix = np.arange(x1 - x0 + 1)[:, None, None]
    iy = np.arange(y1 - y0 + 1)[None, :, None]
    iz = np.arange(z1 - z0 + 1)[None, None, :]
    d = np.full(v.shape, 1 << 30, np.int64)
    for name, dist in (('left', ix), ('right', x1 - x0 - ix), ('front', iy), ('back', y1 - y0 - iy),
                       ('bottom', iz), ('top', z1 - z0 - iz)):
        if name not in open:
            d = np.minimum(d, dist)
    v[d < t] = V.kid(m)


def ellipse(V, x0, x1, y0, y1, z0, z1, m, t=0):
    """Round column fitted in the box: filled disc (t=0) or ring of wall thickness t, extruded over z0..z1."""
    if x1 < x0 or y1 < y0 or z1 < z0:
        return
    v = V.view(x0, x1, y0, y1, z0, z1)
    cx, cy = (x0 + x1) / 2, (y0 + y1) / 2
    rx, ry = (x1 - x0 + 1) / 2, (y1 - y0 + 1) / 2
    X, Y = np.ogrid[x0:x1 + 1, y0:y1 + 1]
    keep = ((X - cx) / rx) ** 2 + ((Y - cy) / ry) ** 2 <= 1
    inner = keep if t else np.zeros_like(keep)
    for _ in range(t):          # erode by a 3x3 square so the ring stays 6-connected (no diagonal gaps)
        p = np.pad(inner, 1)
        inner = np.logical_and.reduce([p[1 + i:p.shape[0] - 1 + i, 1 + j:p.shape[1] - 1 + j]
                                       for i in (-1, 0, 1) for j in (-1, 0, 1)])
    v[(keep & ~inner)[:, :, None] & np.ones(v.shape, bool)] = V.kid(m)
