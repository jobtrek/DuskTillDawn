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
