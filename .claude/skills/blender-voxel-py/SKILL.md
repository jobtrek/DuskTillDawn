---
name: blender-voxel-py
description: Build a detailed, realistic, hard-edged blocky 3D model (character, prop, device, furniture, creature) with its materials using Blender's Python API (the `bpy` module, headless, run with uv), saved as a .blend and optionally exported as a Godot-ready .glb. Use when asked for a voxel model with the Python API / bpy / headless, or when the blender MCP window is not wanted.
---

Mirror of `blender-voxel`: same style rules, same shape helpers, same `build()`. The only difference is where the code runs: here it runs in **headless Blender through the `bpy` PyPI module** (uv), and the deliverable is a saved `.blend`. No Blender window and no MCP add-on are needed.

Deliverable: **one mesh object plus its materials**, saved to a `.blend` (and, only if asked, a `.glb`). The scene holds nothing else: `new_scene()` starts from an empty factory scene, so there is no cube, camera, light or world, and never a ground plane.

## Style (applies to every step)

Identical to `blender-voxel`.

- **Hard edges only.** Every part is built from boxes. No rounded corners, no bevels, no subdivision, no smooth shading (`build` writes flat-shaded faces). Round things are stepped boxes. `blob` and `disc` only for organic parts (heads, fruit, leaves).
- **Real proportions.** Real-world dimensions (a 14" laptop is ~32 × 22 × 1.8 cm closed).
- **Three detail layers.** (1) primary forms, (2) secondary parts: bezels, hinges, buttons, handles, (3) tertiary detail: individual keys, ports, vents, screws, seams, labels.
- **Real colour breakdown.** 15–30 material keys: separate keys for trim, seams, glass, screen, rubber, metal, labels. Every part its own readable colour (no two neighbouring parts the same hue) plus a highlight/shadow variant for faces, glass and metal. A 1-voxel seam or recessed gap sells realism more than extra bulk.

## Setup (once)

The project lives in this skill's folder: `.claude/skills/blender-voxel-py/` (`pyproject.toml`, uv lockfile). `bpy` 5.2.x only ships wheels for **Python 3.13**, so the project pins `3.13` (the system Python may be newer). First run:

```
uv sync --project .claude/skills/blender-voxel-py
```

Never `pip install`. Never call a system `blender` binary. Check docs with `npx ctx7@latest docs /websites/blender_api/4.5 "<question>"` before using any `bpy` API this skill does not already wrap.

## Steps

1. **Design the palette and grid.** Same as `blender-voxel`: 15–30 keys with linear RGB (0–1). +z up, -y is the front, model centred on x/y with feet on z=0.
   - `VOXEL_SIZE` = 1/64 m: characters, doors, furniture, large props.
   - `FINE_SIZE` = 1/128 m: hand-held props and anything with small parts.
   - Smallest feature (key, port, button) at least 2 voxels; seams may be 1. Change resolution with `rescale(V, f)`, never `size=` alone.
   - Write each part's real size converted to a voxel range, with a key. Done when every part has a range and a key.
2. **Build it in one script, run headless.** Write the script to a file in the scratchpad (not the repo), then run it:

   ```
   uv run --project .claude/skills/blender-voxel-py python <script.py>
   ```

   Start the script with the helper, then `new_scene()`, fill `V` (large forms first, details last; later writes overwrite earlier ones), call `build`, then save:

   ```python
   exec(open('/ABS/PATH/.claude/skills/blender-voxel-py/voxel.py').read())
   new_scene()
   V = {}
   box(V, -21, 20, -14, 13, 0, 1, 'chassis')
   tile(V, -18, -17, -11, -10, 2, 2, 'key', nx=12, ny=4, dx=3, dy=3)
   hollow(V, -21, 20, 12, 13, 2, 27, 'bezel', t=1)
   box(V, -19, 18, 12, 12, 4, 25, 'screen')
   r = build('Laptop', V, {'chassis': (.05, .05, .05), ...}, size=FINE_SIZE, grain=0)
   print(r)
   print(save_blend('/ABS/OUT/laptop.blend'))
   ```

   Shape helpers (identical to `blender-voxel`): `box`, `hollow`, `tile`, `blob`, `disc`, `mirror_x`, `carve`, `rescale`. `build` options (identical, minus `remove_cube`): `materials='palette'|'separate'`, `mesh='greedy'|'faces'|'dissolve'`, `collider=True`, `grain=0.08`, `replace=True`, `metallic=(keys)`.

   It returns `{object, voxels, faces_naive, faces, materials, height_voxels, size, height_m, warnings, collider}`. Done when it returns without error, `height_m` matches the real-world height and `warnings` is empty.

   `bpy` prints a `Material.use_nodes` DeprecationWarning; ignore it.
3. **Look at it from several angles.** Call `preview('Laptop', '/ABS/OUT/prev')` in the same script after `save_blend`. It renders `prev_front.png`, `prev_34.png` and `prev_close.png` with Workbench in a throwaway scene (the saved model scene gets no camera or light). Read each PNG with the Read tool. Fix floating voxels, hidden parts, missing detail layers or wrong proportions by editing the script and re-running it (the fresh `new_scene()` replaces everything). Done when each view reads as the requested real-world object and no detail layer is missing.

## Export (only when asked)

`export_glb('Laptop', '/ABS/OUT/laptop.glb')` writes the object (and its collider child). Blender -Y front exports as glTF +Z (Godot's model front); 1 Blender unit is 1 Godot unit; origin at the feet. `save_blend` and `export_glb` accept relative or absolute paths and create missing folders. Never export unprompted; the `.blend` save is part of the deliverable.

## Report

Object name, voxel count, height in metres, the material list, the `.blend` path. Offer `.glb` export as the next action.
