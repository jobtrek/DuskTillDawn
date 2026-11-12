# Agents GUI guide

The look and feel of every in-game 2D UI in DuskTillDawn: menus, HUD, prompts, popups, dialogs, inventories, app screens.

Read this before you create or change any UI. Every component should feel like it belongs to the same game, even when it has a different purpose.

Reference implementation: `dusk-till-dawn/main/computer_ui/` (the desk computer). It is one example of the style. Its desktop/OS look is specific to that computer; the rules below are what carries over.

## Core idea

The UI is part of the voxel world. It is chunky, pixel-crisp, framed like a physical object, and it appears with a short, bouncy motion and blocks popping away.

## Design principles

1. **Pixel art, always crisp.** Draw textures tiny (16×16 icons, 48×48 frames, ~160×90 backgrounds) and scale them up. Set `texture_filter = 1` (Nearest) on the root Control so children inherit it. No blurry, high-res or vector-smooth art.
2. **Voxel-world material.** Panels look like physical blocky objects: a pixel-art frame (wood by default, `frame.png` as a NinePatch with 16px margins) around a content area inset by the same 16px. Other materials (metal, stone) are fine if drawn in the same tiny pixel style.
3. **Blocks are the signature.** When a panel appears, its content is covered with dark voxel blocks that clear from the centre outward (`BlockReveal`). Use it on every panel bigger than a small prompt.
4. **Short, bouncy motion.** Panels rise in from below with a `Back` ease, scale up from ~0.6 and straighten from a slight tilt; the backdrop fades in. Under 0.5s to open, under 0.7s for the reveal. Small things (prompts, toasts) just fade or pop with a quick scale.
5. **Hover feedback on everything clickable.** Scale to ~1.12 and brighten slightly over 0.08s; return on exit.
6. **Dark, calm base.** Near-black navy backgrounds, white text, one or two accent colours from the content itself. Dim the 3D world behind full-screen UI with a 60% black backdrop.
7. **Readable text.** Small sizes (13 for chrome/secondary, 14 for labels). White, with a 2px black drop shadow when text sits on a picture or the 3D world.
8. **Responsive by anchors.** Size panels in percent of the screen with anchors. Use pixel offsets only for fixed inner details (frame inset, bar height). The project stretches with `canvas_items` + `expand`.
9. **Decor never blocks input.** Overlays drawn on top (reveal blocks, glows, vignettes) set `mouse_filter = Ignore`.
10. **UI emits, the game decides.** A UI component exposes C# signals for its actions; other scripts connect to them. Showing/hiding is owned by the trigger (e.g. a zone script), not by the UI itself.

## Palette

| Use | Colour |
|---|---|
| Backdrop over 3D world | `#000000` at 60% |
| Bars / panel fills | `Color(0.06, 0.06, 0.08)` |
| Reveal blocks | `Color(0.05, 0.06, 0.1)`, lightened up to 12% per block |
| Frame | warm wood brown (`frame.png`) |
| Text | white; faded/secondary text at 55% opacity |
| Accent | taken from the content (e.g. NixOS blue on the computer) |

## Standard panel structure

Any full panel (menu, dialog, window, inventory):

```
<Component> (Control, full rect, texture_filter = Nearest)
├── Backdrop (ColorRect, black 60%)          only for full-screen UI
└── Panel (Control, anchored in % of screen) animated as one piece
    ├── Frame (NinePatchRect, frame.png, 16px margins)
    └── Content (Control, clip_contents, inset 16px)
        ├── ...your component's content...
        └── BlockReveal (mouse_filter = Ignore, last child = on top)
```

Small elements (interaction prompts like "press E", toasts, HUD counters) skip the backdrop and reveal, but keep the pixel font sizes, shadowed text and a small frame or dark fill.

## Reusable pieces

| Piece | Where | Use for |
|---|---|---|
| `BlockReveal.cs` | `computer_ui/` | The block-clearing reveal. Set `Progress` 0→1. `Columns` and `BlockColor` are exported. |
| `PlayOpen()` pattern | `ComputerMenu.cs` | Copy for the open animation; trigger it on `VisibilityChanged`. Kill the previous tween first. |
| `SetHover()` pattern | `ComputerMenu.cs` | Hover scale + brighten on buttons. |
| `frame.png` | `computer_ui/textures/` | Default panel frame (NinePatch). |

If a second component needs these, move them to a shared folder (e.g. `main/ui/`) instead of copying them.

## Recipe: new UI component

1. Root `Control`, full rect, `texture_filter = 1`.
2. Build the standard panel structure above.
3. Draw any new textures at tiny pixel sizes and put them in the component's `textures/` folder.
4. Add the open animation (`PlayOpen()` pattern) and `SetHover()` on buttons.
5. Expose actions as `[Signal]`s; let the trigger script show/hide the component.
6. Keep animation durations as `[Export]` fields.

## C# conventions

- Get nodes in `_Ready()`, fields declared `= null!`.
- Signals: `[Signal] public delegate void XPressedEventHandler();`, emitted with `EmitSignal(SignalName.XPressed)`.
- Tunables as `[Export]`.
- Tweens: `CreateTween().SetParallel()`; animate offsets, scale, rotation and modulate, not positions set every frame.

## Example: the desk computer

`computer_ui/` applies the style as a diegetic OS desktop (NixOS + GNOME wallpaper, top bar with live clock, app icons with labels). That desktop theme is only for the computer. Other components keep the style above, not the desktop.

| File | Role |
|---|---|
| `computer_menu.tscn` | Backdrop, framed screen, desktop, three app icons |
| `ComputerMenu.cs` | Open animation, clock, hover, `MailPressed` / `MonitoringPressed` / `HelpPressed` signals |
| `BlockReveal.cs` | Block reveal |
| `../DetectPlayerNearComputer.cs` | Trigger: shows "press E" when the player is in range, opens on E, hides on exit |
