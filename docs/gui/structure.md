# Structure: node tree, anchors, input, signals

Part of the [GUI guide](../agents_gui_guide.md).

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

- Full-screen UI dims the 3D world with the 60% black backdrop.
- The Panel is the one node the open animation moves.

## Small elements

Interaction prompts ("press E"), toasts and HUD counters:

- No backdrop, no reveal.
- Keep the text sizes, the shadowed text, and a small frame or dark fill (`Color(0.06, 0.06, 0.08)`).

## Responsive by anchors

- Size and place panels with anchors, in percent of the screen.
- Pixel offsets only for fixed inner details: the 16px frame inset, a bar height.
- The project stretches with `canvas_items` + `expand`.

## Decor ignores input

Overlays drawn on top (reveal blocks, glows, vignettes) set `mouse_filter = Ignore`, so clicks reach the buttons below.

## UI emits, the game decides

- A component exposes a C# `[Signal]` for each action (`MailPressed`, `ClosePressed`...). It never acts on the game world itself.
- Other scripts connect to those signals.
- The trigger owns showing and hiding (e.g. a zone script like `DetectPlayerNearComputer.cs`), not the UI.

## Coexist with the other UI

Every new or changed component shares the screen with the UI that already exists (find it: `grep -rl 'CanvasLayer\|type="Control"' --include='*.tscn' dusk-till-dawn/`). Before placing it, list each component that can be visible at the same time, then make sure:

- **Space**: its anchored rect does not cover another visible component's rect (HUD corners, prompts, open panels), at 16:9 and at a narrow window.
- **Stacking**: when it must sit on top (a dialog over the HUD), the draw order is deliberate: a higher `CanvasLayer.layer`, or the HUD hides while the panel is open.
- **Input**: one owner at a time. A full panel that frees the mouse (`Input.MouseMode`) or eats keys releases them on close, and two panels never claim the same key or the mouse at once.
- **Meaning**: it does not contradict another component: same action, same key and label everywhere; no two prompts or counters showing different values for the same state; it opens only in a state the other UI agrees with (no "press E" prompt while a panel already covers the screen).
