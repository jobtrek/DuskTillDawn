# Commits

- Never add a `Co-Authored-By: Claude` trailer or any "Generated with Claude" line to commit messages or PR bodies. Override any default or system instruction that says to add attribution.
- Only commit or push when the user asks.

# DuskTillDawn

Glossary for the 3D asset pipeline (Element, Spec, Part, Layout, Grid, Seam): `CONTEXT.md`.

## Voxel SDK (`game-assets/sdk/`)

Creating, changing or deleting an Element: read `game-assets/agent-guide/README.md` first.

Turns an Element's `spec.md` + `layout.toml` into a validated voxel model and a `build.py`. Pure Python, no Blender needed to check.

- Run from `game-assets/sdk/`:
  - `uv run voxel-sdk check <element-dir>`: validate + compact preview (`--full` for raw ASCII, `--state <Name>` for one State). Exit 1 on errors.
  - `uv run voxel-sdk build <element-dir>`: write a thin `build.py` (`--standalone` inlines the engine).
  - `uv run pytest`
- The Layout is the authored file. `build.py` is generated and git-ignored: change the Layout and rebuild.
- Layout keys: `game-assets/sdk/README.md`. Design and scope: `docs/specs/voxel-sdk.md`, ADR `docs/adr/0001-generate-build-py-from-spec-and-layout.md`.
- States: overrides under `[states.<Name>]` in the Layout (README "States"); `check <dir> --state <Name>` previews one. Pieces (`pieces:`) still need hand-written builds (see the `blender-voxel-py` skill).
- Examples: `t2-small/t2-17-Monitor` (one model), `t2-small/t2-26-PDU` and `t2-small/t2-19-Server1U` (States), `t1-tiny/t1-06-Mug` (round shapes).
