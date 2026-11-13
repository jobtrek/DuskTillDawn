---
name: create-ui
description: Build or review DuskTillDawn in-game UI (menu, HUD, prompt, popup, dialog, app screen) in the project's voxel pixel-art style. Use when creating a new UI component, or when the user names an existing UI to check, analyse or fix against the style.
---

# create-ui

The style lives in `docs/agents_gui_guide.md` and the files under `docs/gui/`. This skill picks the branch and holds you to its done-condition.

## Pick the branch

- The user names an existing UI (a scene, script, folder, or "the mail app") → **Review**.
- Otherwise → **Build**.
- Both ("make X and fix Y") → Review Y first, then Build X.

## Review

1. Read `docs/agents_gui_guide.md`, then `docs/gui/review.md`.
2. Find the component: its `.tscn`, every script it attaches, its `textures/` folder, and the trigger script that shows it. Then find every other UI that can be on screen with it (check 15 needs them).
3. Apply all 16 checks in `review.md`. Open the linked rule file whenever a check needs the exact value.
4. Report the table from `review.md`, failures first. Done when every check has a verdict with evidence.
5. Fix only when the user asks. After fixing, `dotnet build` in `dusk-till-dawn/` and re-run the failed checks.

## Build

1. Read `docs/agents_gui_guide.md`, then `docs/gui/new-component.md` and every file it links.
2. Pin down five facts before writing: what it is for, full panel or small element, the signals it emits, the script that shows it, and which existing UI can be on screen with it. Ask the user only for facts the request and specs (`docs/specs/`) leave open.
3. Follow the steps in `new-component.md`, using `dusk-till-dawn/main/computer_ui/` as the code to copy patterns from.
4. Done when `dotnet build` passes and all 16 checks in `docs/gui/review.md` pass on the new component. Report the check table.
