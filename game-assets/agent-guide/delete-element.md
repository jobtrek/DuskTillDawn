# Delete or rename an Element

Ask the user before deleting. It removes the only copy of the model if the Element is hand-written.

1. Find references: `grep -rn "<Name>\|<id>" --include='*.md' --include='*.toml' --include='*.py' --include='*.tscn' --include='*.cs' /Users/jobtrekwork/DuskTillDawn | head`. Check `Room_*` Specs and `docs/`.
2. Delete the folder `game-assets/<tier>/<id>-<Name>/` (`git rm -r`; the ignored generated `build.py` goes with the folder).
3. Delete the exported `.glb` and any Godot scene or script that loads it.
4. Regenerate the index from the repo root: `uv run scripts/build_index.py`. Never edit the tables in `docs/3d-assets.md` by hand.
5. Rename: move the folder, update `name:` and `id:` in the Spec, update references from step 1, rebuild.
