# DuskTillDawn

Glossary for the 3D asset pipeline (Element, Spec, Part, Layout, Grid, Seam): `CONTEXT.md`.

## Voxel SDK (`game-assets/sdk/`)

Turns an Element's `spec.md` + `layout.toml` into a validated voxel model and a self-contained `build.py`. Pure Python, no Blender needed to check.

- Run from `game-assets/sdk/`:
  - `uv run voxel-sdk check <element-dir>`: validate + ASCII preview (front/side/top). Exit 1 on errors.
  - `uv run voxel-sdk build <element-dir>`: write `build.py`.
  - `uv run pytest`
- The Layout is the authored file. `build.py` is generated: change the Layout and rebuild.
- Layout keys: `game-assets/sdk/README.md`. Design and scope: `docs/specs/voxel-sdk.md`, ADR `docs/adr/0001-generate-build-py-from-spec-and-layout.md`.
- States: overrides under `[states.<Name>]` in the Layout (README "States"); `check <dir> --state <Name>` previews one. Pieces (`pieces:`) still need hand-written builds (see the `blender-voxel-py` skill).
- Examples: `t2-small/t2-17-Monitor` (one model), `t2-small/t2-26-PDU` and `t2-small/t2-19-Server1U` (States), `t1-tiny/t1-06-Mug` (round shapes).
