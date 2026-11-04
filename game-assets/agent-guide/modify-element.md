# Modify an Element

1. Check the Element has a `layout.toml`. No? Read [hand-written.md](hand-written.md) instead.
2. Edit the right file:
   - Size or colour of a Part: its line in `spec.md`.
   - Position, repeat, shape, State override: `layout.toml`.
   - Add a Part: a line in the Spec **and** a table in the Layout (same name).
   - Remove a Part: delete both. Parts that sat `on` it must be re-anchored.
3. `uv run voxel-sdk check <dir>`. For States also `check <dir> --state <Name>`.
4. `uv run voxel-sdk build <dir>`.
5. If the overall size changed, update `size_cm:` and `voxels:` together, and ask the user first.
6. Run `uv run voxel-sdk index` from `game-assets/sdk/` if the Spec changed.

Tips:
- Rename a Part with find and replace across `spec.md`, `layout.toml` and every `[states.<Name>]` table.
- States: `states:` in the Spec names them; the first is the base. Overrides live in `[states.<Name>]`.
  `omit = true` hides a Part; `[[states.<Name>.raw]]` runs only in that State.
- Overlap on purpose with a negative `gap` or an `offset_*`. Same-colour neighbours only warn.
