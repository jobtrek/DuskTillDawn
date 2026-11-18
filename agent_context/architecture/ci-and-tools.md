# CI and tools

## Requirements

Godot 4.7.2 (.NET build), .NET SDK 10.0.403, Git LFS, `uv` for Python.

## CI (`.github/workflows/ci.yml`, on PRs to `main`)

Runs in `dusk-till-dawn/` on `DuskTillDawn.sln`:

1. `dotnet restore`
2. `dotnet build`
3. `dotnet test` (game-core.tests)
4. `dotnet format --verify-no-changes`

Run steps 2–4 locally before saying C# work is done.

## Scripts (`scripts/`)

| Script | Use |
|---|---|
| `run-fast.sh [speed]` | Build and launch the game with the clock sped up (default ×20) |
| `gen_computer_ui_textures.py` | Regenerate desk computer pixel textures |
| `migrate_specs.py` | One-off spec migration, already run. Do not rerun |

## Issue templates (`.github/ISSUE_TEMPLATE/`)

Epic, user story, task, bug report. User stories are vertically sliced and must
match `docs/contract.md`.

## Project skills (`.claude/skills/`)

| Skill | Use |
|---|---|
| `create-ui` | Build or review in-game UI |
| `blender-voxel-py` | Build a voxel model in headless Blender |
| `csharp-godot` | C# in Godot API reference |
