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

- [1] closed body 33 x 22 x 1.6 cm (lid chassis over metal base)
- [2] hinge 2x 5.5 x 1.6 x 1.6 cm at the back, metal_dark
- [3] status LED 2x2 voxels (front edge), led_green / led_orange / led_red / led_off
- [3] vents 6 slots on the left edge, seam (orange when Hot)
- [3] rubber feet 4x 2.3 x 1.6 x 0.8 cm, rubber
- [3] power port 2.3 x 1.6 cm on the left edge, plastic_black
- [3] label (logo plate) 3 x 3 cm on the lid, trim
