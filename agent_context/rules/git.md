# Git rules

- **Commit or push only when the user asks.**
- Never add `Co-Authored-By: Claude` or "Generated with Claude" to commits or PRs.
- On `main`: create a branch before committing.
- Never commit generated files: `build.py` from the voxel SDK, `bin/`, `obj/`, `.godot/`.
- Never commit secrets (`.env`, credentials, `settings.local.json`).
- Large binaries (`.blend`, `.glb`, `.png`) go through Git LFS.
