# Look: textures, frame, colours, text

Part of the [GUI guide](../agents_gui_guide.md).

## Pixel art, always crisp

- Draw textures tiny and scale them up: 16×16 icons, 48×48 frames, ~160×90 backgrounds.
- Set `texture_filter = 1` (Nearest) on the root Control. Children inherit it.
- Every texture is pixel art. Smooth, high-res or vector art breaks the look.
- New textures go in the component's own `textures/` folder.

## Voxel-world material

Panels look like physical blocky objects:

- A pixel-art frame around the content. Default: wood, `computer_ui/textures/frame.png` as a `NinePatchRect` with 16px margins.
- The content area is inset by the same 16px.
- Other materials (metal, stone) are fine when drawn in the same tiny pixel style.

## Palette

| Use | Colour |
|---|---|
| Backdrop over 3D world | `#000000` at 60% |
| Bars / panel fills | `Color(0.06, 0.06, 0.08)` |
| Reveal blocks | `Color(0.05, 0.06, 0.1)`, lightened up to 12% per block |
| Frame | warm wood brown (`frame.png`) |
| Text | white; faded/secondary text at 55% opacity |
| Accent | one or two, taken from the content (e.g. NixOS blue on the computer) |

## Text

- Size 13 for chrome and secondary text, 14 for labels.
- White. Secondary text at 55% opacity.
- A 2px black drop shadow when text sits on a picture or on the 3D world.
