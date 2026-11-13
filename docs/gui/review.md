# Review an existing UI component

Part of the [GUI guide](../agents_gui_guide.md). Read the component's `.tscn`, its scripts, and its `textures/` folder, then apply every check below. Done when each check has a verdict: **pass**, **fail** (with `file:line` and the fix), or **n/a** (with the reason, e.g. "small element, no reveal").

## Look ([look.md](look.md))

1. Root has `texture_filter = 1`.
2. Every texture is tiny pixel art (open the PNGs: icons ~16×16, frames ~48×48, backgrounds ~160×90).
3. Content sits in a pixel-art frame (`NinePatchRect`, 16px margins) or, for small elements, a small frame or dark fill. A plain `StyleBoxFlat` border counts as a fail on full panels.
4. Colours match the palette: fills `Color(0.06, 0.06, 0.08)`, white text, accents from the content.
5. Text is 13 or 14, white or 55% white, shadowed (2px black) on pictures and the 3D world.

## Motion ([motion.md](motion.md))

6. Full panels play the `PlayOpen()` animation on `VisibilityChanged`, killing the previous tween first.
7. Full panels end with a `BlockReveal` tweened 0 → 1.
8. Every clickable node has the hover zoom (1.12, brighten, 0.08s).
9. Durations are `[Export]` fields, open < 0.5s, reveal < 0.7s.

## Structure ([structure.md](structure.md))

10. Node tree follows the standard panel structure; full-screen UI has the 60% backdrop.
11. Panels are sized with anchors in percent; pixel offsets only for inner details.
12. Overlays (reveal, glows, vignettes) have `mouse_filter = Ignore`.
13. Actions leave the component only as `[Signal]`s; the component never hides itself or touches game state.
14. Shared pieces (`BlockReveal`, `frame.png`, `PlayOpen`, `SetHover`) are referenced, not copied. A second user of a piece still living in `computer_ui/` is a fail: move it to `main/ui/`.

## Code

15. C# conventions from [new-component.md](new-component.md#c-conventions) hold.

## Report

A table: check number, verdict, `file:line`, fix. Failures first. Change files only when the user asks for fixes.
