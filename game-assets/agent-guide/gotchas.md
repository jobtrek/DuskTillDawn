# Gotchas

| Symptom | Cause and fix |
|---|---|
| `size x: model is N voxels, Spec says M` | The Layout or the Spec is wrong beyond 1 voxel. Fix the model; ask before editing `voxels:`. |
| `floating Part(s)` | A Part touches nothing. Anchor it with `on`, or use a negative `gap` so it overlaps its host. |
| `unknown palette key` | Key not in `palette.py`. Add it to the Layout's `[palette]` table (linear RGB), or pick another key. |
| Spec line parses as 4 numbers | `2x 0.8 x 1.6 x 1.6 cm`. Write `0.8 x 1.6 x 1.6 cm, 2 copies, key`. |
| A raw block carved away later Parts | Raw blocks run after all Parts. Put the later shapes in the same raw block, or use a hollow Part. |
| `build` printed "hand-written model" | The folder has no `layout.toml`. See [hand-written.md](hand-written.md). |
| `build.py` changes do not stick | It is generated. Change the Layout and rebuild. |
| Generated `build.py` does not run in Blender | Thin builds read the engine from the repo. Use `build --standalone`. |
| `uv run pytest` fails after an SDK edit | Run it from `game-assets/sdk/`. Read the failing test before changing the code. |

Axes: +z up, -y front, first Part centred on x/y at z=0. Grids: S = 1.5625 cm per voxel, F = 0.78125 cm.
Do not touch `game-assets/final-built-assets/` unless asked: those are saved Blender and glb outputs.
