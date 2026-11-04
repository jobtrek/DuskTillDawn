---
name: RackFrame
id: t3-13
tier: T3
room: Basement
priority: V1
size_cm: 60x80x120
voxels: 77x102x154
grid: F
grain: 0
collider: true
states:
  Ok: top LED `led_green`
  Warn: top LED `led_orange`
  Failed: top LED `led_red`
---

# RackFrame

## Prompt

### Shape
Build a voxel RackFrame for the Basement room, about 60x80x120 cm (77x102x154 voxels). Main forms: posts, side panels, door.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: rail holes, vent slots, casters, lock, label.

## Parts (tier; cm size + colour key to fill before first build)

- [2] casters: 4 x 4 x 4 cm, rubber
- [2] posts: 3 x 3 x 114 cm, chassis_dark
- [2] top plate: 60 x 80 x 2 cm, chassis
- [2] side panels: 0.8 x 74 x 104 cm, chassis
- [2] door: 56 x 1.6 x 108 cm, metal_dark
- [2] rail holes: 0.8 x 0.8 x 1.6 cm, seam
- [2] vent slots: 40 x 0.8 x 1 cm, chassis_dark
- [2] lock: 3 x 0.8 x 4 cm, metal
- [2] label: 8 x 0.8 x 3 cm, label
- [3] top-of-rack LED: 3.1 x 0.8 x 3.1 cm (Ok green / Warn orange / Failed red), led_green
