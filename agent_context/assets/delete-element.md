# Delete or rename an Element

Ask the user before deleting. It removes the only copy of the model if the Element is hand-written.

1. Find references with `rg` (ripgrep) or `sgrep`, never plain `grep -r`. Run from the repo root, once for the Name and once for the id:
   `rg -n -i "<Name>|<id>" --glob '!game-assets/final-built-assets' --glob '!**/build.py'`
   Check the hits in `Room_*` Specs, `docs/`, Godot scenes (`.tscn`) and scripts (`.cs`). A Spec `id:` line in the Element's own folder is expected.
2. Delete the folder `game-assets/<tier>/<id>-<Name>/` (`git rm -r`; the ignored generated `build.py` goes with the folder).
3. Delete the exported `.glb` and any Godot scene or script that loads it.
4. Regenerate the index: `uv run voxel-sdk index` from `game-assets/sdk/`. Never edit `game-assets/INDEX.md` by hand.
5. Rename: move the folder, update `name:` and `id:` in the Spec, update references from step 1, rebuild.
