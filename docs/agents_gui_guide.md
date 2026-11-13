# Agents GUI guide

The look and feel of every in-game 2D UI in DuskTillDawn: menus, HUD, prompts, popups, dialogs, inventories, app screens.

Read this page first. Open only the file your task needs.

| Task | Read |
|---|---|
| Build a new UI component | [gui/new-component.md](gui/new-component.md) |
| Check an existing UI against the style | [gui/review.md](gui/review.md) |
| Textures, frame, colours, text | [gui/look.md](gui/look.md) |
| Open animation, block reveal, hover | [gui/motion.md](gui/motion.md) |
| Node tree, anchors, input, signals | [gui/structure.md](gui/structure.md) |
| See the style applied (the desk computer) | [gui/computer-example.md](gui/computer-example.md) |

Skill: `/create-ui` runs both the build and the review flow.

## Core idea

The UI is part of the voxel world. It is chunky, pixel-crisp, framed like a physical object, and it appears with a short, bouncy motion and blocks popping away. Every component should feel like it belongs to the same game, even when it has a different purpose.

## Ten rules

1. **Crisp pixel art**: tiny textures scaled up, `texture_filter = 1` (Nearest) on the root. ([look](gui/look.md))
2. **Voxel material**: a pixel-art frame around the content, like a physical block. ([look](gui/look.md))
3. **Blocks reveal**: every panel bigger than a small prompt opens with `BlockReveal`. ([motion](gui/motion.md))
4. **Bouncy open**: rise from below with a `Back` ease, under 0.5s. ([motion](gui/motion.md))
5. **Hover zoom**: anything clickable scales to ~1.12 and brightens. ([motion](gui/motion.md))
6. **Dark calm base**: near-black navy, white text, accents from the content. ([look](gui/look.md))
7. **Readable text**: sizes 13 and 14, 2px black shadow on pictures and the 3D world. ([look](gui/look.md))
8. **Anchors**: size panels in percent of the screen. ([structure](gui/structure.md))
9. **Decor ignores input**: overlays set `mouse_filter = Ignore`. ([structure](gui/structure.md))
10. **UI emits, the game decides**: signals out; the trigger shows and hides. ([structure](gui/structure.md))

The OS desktop look (wallpaper, top bar, app icons) belongs only to the desk computer. Other components keep the ten rules, not the desktop.
