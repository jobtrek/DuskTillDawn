---
name: RouterBox
id: t2-13
tier: T2
room: Basement
priority: UP
size_cm: 44x25x4.4
voxels: 56x32x6
grid: F
grain: 0
collider: false
states:
  On: status LED `led_green`, port LEDs `led_green`
  Hot: status LED `led_orange`
  Failed: status LED `led_red`, port LEDs `led_off`
---

# RouterBox

## Prompt

### Shape
Build a voxel RouterBox for the Basement room, about 44x25x4.4 cm (56x32x6 voxels). Main forms: case, ear brackets.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: ports, LEDs, vents, label.

## Parts (tier; cm size + colour key to fill before first build)

- [1] case 44 x 25 x 4.4 cm, chassis
- [2] ear brackets 2x 2.3 x 12 x 3.9 cm, metal
- [3] ports 8x tile 2.3 x 1.6 cm, screen
- [3] status LED 2x2 + port LEDs 4x 2x2, led_green / led_orange / led_red / led_off
- [3] vents 7 slots on top, seam (glow led_orange when Hot)
- [3] label 7.8 x 4.7 cm on top, label
