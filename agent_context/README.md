# Agent context

Everything an agent needs to work on DuskTillDawn. One topic per file, 40 lines max.
Read this index, then open only the file your task needs. Terms: [CONTEXT.md](../CONTEXT.md).

## Always

| Topic | Read |
|---|---|
| Writing C# / Godot code | [rules/code.md](rules/code.md) |
| What agents may and may not write | [rules/ai-usage.md](rules/ai-usage.md) |
| Commits, PRs, branches | [rules/git.md](rules/git.md) |
| Writing a user story or task | [rules/user-stories.md](rules/user-stories.md) |

## By task

| Task | Read |
|---|---|
| Understand the game | [game/overview.md](game/overview.md) |
| Game rules (time, SLA, simulation, Incidents) | [game/rules-v1.md](game/rules-v1.md) |
| Where do I change X? | [architecture/where-to-change.md](architecture/where-to-change.md) (UI: [where-to-change-ui.md](architecture/where-to-change-ui.md)) |
| Find where code lives | [architecture/overview.md](architecture/overview.md) |
| Godot project (scenes, UI, controllers) | [architecture/godot-project.md](architecture/godot-project.md) |
| Game logic library | [architecture/game-core.md](architecture/game-core.md) |
| 3D voxel models pipeline | [architecture/asset-pipeline.md](architecture/asset-pipeline.md) |
| CI, scripts, tools | [architecture/ci-and-tools.md](architecture/ci-and-tools.md) |
| Create / change / delete an Element (model) | [assets/README.md](assets/README.md) |
| Create / change in-game UI | [gui/README.md](gui/README.md) or the `create-ui` skill |

## Sources of truth (human-owned, read-only for agents)

- `docs/contract.md`: what v1 is. `docs/game-v1.md`: v1 numbers and rules.
- `docs/adr/`: decisions. `docs/specs/`: app specs. `docs/ideas/`: parked, do not build.
