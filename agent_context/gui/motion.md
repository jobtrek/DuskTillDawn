# Motion: open animation, block reveal, hover

Part of the [GUI guide](README.md). Call `dusk-till-dawn/main/UiMotion.cs`, never copy its tweens: `AddHover`, `PlayLiftOpen`, `PlayWindowOpen`, `TryConsumeClose` (Esc), `Pop`, `SlideIn`.

## Open animation

Full-screen panels (desk computer desktop, night report) call `UiMotion.PlayLiftOpen`. App windows (Mail, Monitoring, Admin) call `UiMotion.PlayWindowOpen`. Each is one parallel tween:

| Property | Lift open | Window open | Ease |
|---|---|---|---|
| Panel offset (top + bottom) | 60% of viewport height below → 0 | not moved | `Back` / `Out` |
| Panel scale | ~0.6 → 1 | 0.85 → 1 | `Back` / `Out` |
| Panel rotation | ~-8° → 0 | not rotated | `Cubic` / `Out` |
| Modulate | backdrop transparent → white | window transparent → white | linear |
| `BlockReveal.Progress` | 0 → 1, from half the open | 0 → 1, from 40% of the open | linear |

- Open under 0.5s (`OpenDuration = 0.45`), reveal under 0.7s (`RevealDuration = 0.6`). Both are `[Export]` fields.
- Trigger it from `VisibilityChanged` when `IsVisibleInTree()`.
- Store the returned tween in `_openTween` and pass it back next time: `UiMotion` kills it and resets every animated property first.
- `PlayLiftOpen` sets the pivot to bottom-centre when the panel opens, so it lifts like a map. App windows keep their pivot centred on `Resized`.

## Block reveal (`BlockReveal.cs`)

The signature of the style. When a panel appears, its content is covered with dark voxel blocks that clear from the centre outward.

- Use it on every panel bigger than a small prompt.
- Last child of the content area, so it draws on top. `mouse_filter = Ignore`.
- Drive it by tweening `Progress` 0 → 1. `Columns` (default 24) and `BlockColor` are exported.

## Hover (`UiMotion.AddHover`)

Call it once on everything clickable:

- On enter: scale to 1.12 and `self_modulate` to `Color(1.25, 1.25, 1.25)` over 0.08s.
- On exit: back to scale 1 and white over 0.08s.
- It sets `PivotOffset = Size / 2` itself, so it zooms from the centre.

## Small elements

Prompts ("press E"), toasts and HUD counters skip the backdrop and reveal. They fade in, or pop with `UiMotion.Pop`.
