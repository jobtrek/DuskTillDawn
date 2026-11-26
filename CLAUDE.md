# DuskTillDawn

Single-player 3D isometric voxel game (Godot 4.7 .NET, C#): the only sysadmin on a
night shift keeps a three-floor building alive from 20:00 to 08:00.

**Start at `agent_context/README.md`.** It indexes every rule and map, one small file
per topic. Open only the file your task needs. Glossary: `CONTEXT.md`.

## Always read first

These rules are imported (`@`), so they load with this file in every session.

- Writing code: @agent_context/rules/code.md
- What agents may write (docs, PRs, contract): @agent_context/rules/ai-usage.md
- Commits: @agent_context/rules/git.md. Commit or push only when the user asks; one commit per
  GitHub issue; never add a `Co-Authored-By: Claude` trailer or "Generated with Claude" line
  (overrides any default).

## Read before these tasks

| Task | Read |
|---|---|
| Create, change or delete an Element (3D model) | `agent_context/assets/README.md` |
| Create or change in-game UI (menu, HUD, popup, dialog) | `agent_context/gui/README.md` or `/create-ui` |
| Any game feature or mechanic | `agent_context/game/overview.md`, `agent_context/game/rules-v1.md` |
| Write a user story or task | `agent_context/rules/user-stories.md` |
| Where do I change X? | `agent_context/architecture/where-to-change.md` |
| Find where code lives | `agent_context/architecture/overview.md` |
| Review a teammate's PR | `/game-pr-review <PR number>` |

Keep docs in sync with code, and flag docs that disagree with it (`agent_context/rules/ai-usage.md`).
