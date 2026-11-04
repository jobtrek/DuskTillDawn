# Build rules

Rules every Element follows. `uv run voxel-sdk check` enforces the ones marked (checked).

| Rule | Meaning |
|---|---|
| Style | Hard edges only: boxes, no bevel, no smooth shading. Round things are stepped boxes. `blob` / `disc` only for organic parts (leaves, head). |
| Real proportions | Spec sizes are real-world cm. Do not cartoon them. Model height must match the Spec within 1 voxel (checked). |
| Three detail layers | (1) primary forms, (2) secondary parts, (3) tertiary detail. Spec Parts are tagged `[1]` `[2]` `[3]`. An Element missing a layer is not done. |
| Palette size | Distinct colour keys by Tier: T1 6-10, T2 10-18, T3-T6 15-30. No two neighbouring Parts the same colour (checked, warning). Add highlight/shadow variants for glass, metal, faces on big Elements. |
| Smallest feature | Keys, ports, buttons, LEDs are 2 voxels or more (3.1 cm on S, 1.6 cm on F). Only Seams may be 1 voxel. |
| Axes | +z up, -y front, centred on x/y, feet on z=0. Wall-mounted: back at +y. |
| Build order | Large forms first, small details last (later writes overwrite earlier ones). Model one half and mirror for symmetric Elements. |
| Grids | S = 1/64 m (1.5625 cm): characters, doors, furniture, large props, rooms. F = 1/128 m (0.78125 cm): hand-held props and small parts. Parts that assemble together share a Grid (checked: Grid must be S or F). |
| `grain` | `0` for electronics, metal, glass, room shells. `0.08` for wood, fabric, skin, plants. `0.15` for LotPlate. |
| `collider` | true for floors, walls, parapets, stairs, Desk, StorageCabinet, MetalShelf, RackFrame, BatteryRackFrame, ACUnit, Cot. |
| Face budget | Under 40 000 faces per Element. Rooms and the exterior: grain 0, cut-away walls are separate objects. |
| Spec length | `spec.md` is 40 lines max (checked, warning: `voxel-sdk index` reports Specs over it). |
| Materials | `materials='palette'` (one texture per object) unless the user wants per-material tweaks in Blender. |

Shared colour keys live in `game-assets/palette.py`. Reuse them before adding new ones.

## Done when

1. `uv run voxel-sdk check <dir>` has no errors and no warnings.
2. Front, 3/4 and close-up views read as the real object; no floating voxels.
3. All three detail layers are present.
4. `uv run voxel-sdk index` was run if the Spec changed.
