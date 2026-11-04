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

- [1] main case: 40.62 x 25 x 4.69 cm, chassis
- [2] rack ear brackets: 1.56 x 9.38 x 3.91 cm, metal
- [2] rack ear brackets 2: 1.56 x 9.38 x 3.91 cm, metal
- [3] dark screw slots: 0.78 x 0.78 x 1.56 cm, 4 copies, seam
- [3] eight ports: 1.56 x 0.78 x 1.56 cm, 8 copies, screen
- [3] port leds above every: 1.56 x 0.78 x 1.56 cm, 4 copies, led_green
- [3] status led 2x2: 1.56 x 0.78 x 1.56 cm, led_green
- [3] seven vent slots: 0.78 x 10.94 x 0.78 cm, 7 copies, seam
- [3] paper label: 7.81 x 5.47 x 0.78 cm, label

