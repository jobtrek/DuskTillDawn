---
name: PDU
id: t2-26
tier: T2
room: Basement
priority: UP
size_cm: 44x10x4.4
voxels: 56x13x6
grid: F
grain: 0
collider: false
states:
  On: status LED `led_green`
  Overload: status LED `led_orange`, load bar full
  Tripped: status LED `led_red`, breaker switch down, outlet LEDs `led_off`
  Off: status LED `led_off`
---

# PDU

## Prompt

### Shape
Build a voxel PDU for the Basement room, about 44x10x4.4 cm (56x13x6 voxels). Main forms: case, ear brackets.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: outlets, status LED, breaker switch, power cord, label.

## Parts (tier; cm size + colour key to fill before first build)

- [1] case 44 x 10 x 4.4 cm, chassis
- [2] ear brackets 2x 1.6 x 10 x 3.9 cm, metal
- [3] outlets 8x (tile) 3 x 2.3 cm, plastic_black with metal pins
- [3] status LED 2x2: led_green / led_orange / led_red / led_off
- [3] breaker switch 3 x 3 cm, plastic_black housing, lever plastic_white (On) / warning_red (Tripped)
- [3] power cord 3 x 1.6 x 3 cm at the back, rubber
- [3] label 7.8 x 3 cm on top, label
