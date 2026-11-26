# Motion: open animation, block reveal, hover

Part of the [GUI guide](README.md). Call `dusk-till-dawn/main/UiMotion.cs` (hover, App window open, HUD pop, lift-the-map open for full-screen panels).

## Open animation (`PlayOpen()` pattern)

Full panels open like this, all in one parallel tween:

| Property | From | To | Ease |
|---|---|---|---|
| Panel offset (top + bottom) | 60% of viewport height below | 0 | `Back` / `Out` |
| Panel scale | ~0.6 | 1 | `Back` / `Out` |
| Panel rotation | ~-8° | 0 | `Cubic` / `Out` |
| Backdrop modulate | transparent | white | linear |
| `BlockReveal.Progress` | 0 | 1 | starts at half the open duration |

- Open under 0.5s (`OpenDuration = 0.45`), reveal under 0.7s (`RevealDuration = 0.6`). Both are `[Export]` fields.
- Trigger it from `VisibilityChanged` when `IsVisibleInTree()`.
- Kill the previous tween first (`_openTween?.Kill()`), then reset every animated property before tweening.
- `PlayLiftOpen` sets the pivot to bottom-centre when the panel opens, so it lifts like a map.

## Block reveal (`BlockReveal.cs`)

The signature of the style. When a panel appears, its content is covered with dark voxel blocks that clear from the centre outward.

- Use it on every panel bigger than a small prompt.
- Last child of the content area, so it draws on top. `mouse_filter = Ignore`.
- Drive it by tweening `Progress` 0 → 1. `Columns` (default 24) and `BlockColor` are exported.

## Hover (`SetHover()` pattern)

Everything clickable:

- On enter: scale to 1.12 and `self_modulate` to `Color(1.25, 1.25, 1.25)` over 0.08s.
- On exit: back to scale 1 and white over 0.08s.
- Set `PivotOffset = Size / 2` first so it zooms from the centre.

## Small elements

Prompts ("press E"), toasts and HUD counters skip the backdrop and reveal. They fade in, or pop with a quick scale.
