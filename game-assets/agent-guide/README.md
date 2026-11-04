# Agent guide: voxel Elements

Read this page first. Open only the file your task needs.

| Task | Read |
|---|---|
| Create a new Element | [new-element.md](new-element.md) |
| Change an Element (size, colour, Part, State) | [modify-element.md](modify-element.md) |
| Delete or rename an Element | [delete-element.md](delete-element.md) |
| Element has no `layout.toml` (hand-written `build.py`) | [hand-written.md](hand-written.md) |
| Rules every Element follows (grids, axes, detail layers, palette) | [build-rules.md](build-rules.md) |
| Something fails or looks wrong | [gotchas.md](gotchas.md) |

Terms (Element, Spec, Part, Layout, Grid, State): `CONTEXT.md`. Layout keys and States: `sdk/README.md`.

## Five rules

1. The **Spec** (`spec.md`) and **Layout** (`layout.toml`) are the source. `build.py` next to them is generated and git-ignored: never edit it, never commit it.
2. Always run `uv run voxel-sdk check <dir>` (from `game-assets/sdk/`) before `build`. Exit 1 means errors; fix them first.
3. An Element folder without `layout.toml` is hand-written. Its `build.py` is the only source: do not run plain `build` on it ([hand-written.md](hand-written.md)).
4. Do not change the Spec's `voxels:` or `size_cm:` to make a check pass without asking the user. A mismatch usually means the model is wrong.
5. Commit or push only when the user asks.

## Commands (run from `game-assets/sdk/`)

```
uv run voxel-sdk check ../<tier>/<id>-<Name>              # validate + compact preview
uv run voxel-sdk check ../<tier>/<id>-<Name> --state On   # one State
uv run voxel-sdk build ../<tier>/<id>-<Name>              # write build.py (thin)
uv run voxel-sdk build --standalone ../<tier>/<id>-<Name> # build.py that runs in Blender without the repo
uv run voxel-sdk index [--check]                           # regenerate game-assets/INDEX.md
uv run pytest                                             # SDK tests
```
