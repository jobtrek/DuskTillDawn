# New Element

1. Copy the closest existing Element folder as a start. Same Tier and Grid if you can.
   Good examples: `t2-small/t2-17-Monitor` (one model), `t2-small/t2-26-PDU` (States), `t1-tiny/t1-06-Mug` (round shapes).
2. Folder name: `game-assets/<tier>/<id>-<Name>/`. The `id` is unique (`t2-27`).
3. Write `spec.md` (30 lines max): front matter, then `## Parts` with one line per Part:
   `- [tier] name: W x D x H cm, colour_key`. Add `, 2 copies` for repeats.
   - Size order is W x D x H. Never write `2x 0.8 x 1.6 x 1.6 cm`: it parses as 4 numbers.
   - Colour keys come from `game-assets/palette.py`. Missing key? Add it under `[palette]` in the Layout (this Element only), or ask before changing `palette.py`.
   - `voxels:` = `size_cm` / voxel size, rounded (S = 1.5625 cm, F = 0.78125 cm). The SDK allows 1 voxel of slack.
4. Write `layout.toml`: one table per Part, in placement order. The first Part has no `on`; every other Part is `on = "<part>.<face>"` (top, bottom, front = -y, back = +y, left = -x, right = +x). Part names must equal the Spec names.
5. `uv run voxel-sdk check <dir>`; read the per-Part table and fix errors (size mismatch, floating Part, unknown key).
6. `uv run voxel-sdk build <dir>`; the user runs `build.py` in Blender (Alt+P).
7. Regenerate the index from the repo root: `uv run scripts/build_index.py` (`--check` verifies).

Build order and detail rules (3 layers, 2-voxel minimum features, +z up, -y front): `docs/3d-assets.md`.
Use `[[raw]]` only when no Part shape fits (`carve`, `blob`, `ellipse`). Raw blocks run after all Parts.
