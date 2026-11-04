---
name: Laptop
id: t2-24
tier: T2
room: Basement
priority: V1
size_cm: 33x23x2.5
voxels: 42x29x3
grid: F
grain: 0
collider: false
states:
  On: LED `led_green`
  Hot: LED `led_orange`, darkened vents
  Failed: LED `led_red`
  Off: LED `led_off`
---

# Laptop

## Prompt

### Shape
Build a voxel Laptop for the Basement room, about 33x23x2.5 cm (42x29x3 voxels). Main forms: closed body, hinge.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: lying flat with the screen side down (keyboard deck up), status LED on the front edge, vents, rubber feet, power port, label.

## Parts (tier; cm size + colour key to fill before first build)

- [1] base: 33 x 22 x 0.8 cm, metal
- [1] lid: 33 x 22 x 0.8 cm, chassis
- [2] hinges: 4.7 x 0.8 x 1.6 cm (2x, back), metal_dark
- [3] status LED: 1.6 x 0.8 x 1.6 cm (front edge; colour per State), led_green
- [3] vents: 0.8 x 0.8 x 0.8 cm (6x, left edge; orange when Hot), seam
- [3] rubber feet: 2.3 x 1.6 x 0.8 cm (4x), rubber
- [3] power port: 0.8 x 3.1 x 0.8 cm (left edge), plastic_black
- [3] logo plate: 3.1 x 3.1 x 0.8 cm (on the lid), trim
