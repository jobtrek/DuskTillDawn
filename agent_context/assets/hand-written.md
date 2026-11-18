# Hand-written Elements (no `layout.toml`)

Some Elements are still a hand-written `build.py`. That file is the **only source** of the model and is tracked in git.

Find them: `git ls-files game-assets | grep build.py`.

## Rules

- Never run plain `voxel-sdk build` on one. The SDK refuses and prints a hint. Do not work around it.
- Edit only the `# 3. MODEL` section (and the `PALETTE` line above it). The top of the file is a loader that reads `sdk/src/voxel_sdk/engine_blender.py`.
- These files need the repo checked out to run in Blender. For a script that runs anywhere:
  `uv run voxel-sdk build --standalone ../<tier>/<id>-<Name>` writes `build.standalone.py` (git-ignored).
- `Room_Roof` still carries its own older engine copy. Do not thin it: the SDK engine fixes lattice offsets, which could change its output.
- Room shells (`Room_*`) call `bpy` to build Pieces. They cannot become Layouts.

## Migrating one to a Layout

Only with the user's agreement. A Layout is smaller and checked, but the model must stay voxel-identical.

1. Keep the old `build.py` somewhere safe (copy it out of the repo).
2. Write the Spec Parts and Layout until `check` passes.
3. Compare old and new: same voxels in every State, same `build()` arguments (`size`, `grain`, `metallic`, `collider`), same colours.
   If any differ, fix the Layout, not the model.
4. Only then `build`, and `git rm --cached` the old tracked `build.py`.

Hard cases, so you do not re-discover them:
- Floating Parts (no face contact with the main body) are an SDK error. Embers or screws in the air need a decision from the user.
- Spec `voxels:` smaller or larger than the hand model by more than 1 voxel is an error.
- Loops that draw 100+ boxes become 100+ Parts. Ask before converting.
- `carve` works as a `[[raw]]` block, but raw runs after all Parts, so Parts that must be drawn after a carve cannot be Parts. A hollow Part (`shape = "hollow"`, `thickness`, `open`) often replaces a carve.
- A State list in the hand model that differs from the Spec's `states:` is a mismatch: ask.
