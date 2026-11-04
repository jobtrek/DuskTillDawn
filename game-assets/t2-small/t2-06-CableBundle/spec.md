---
name: CableBundle
id: t2-06
tier: T2
room: Basement
priority: V1
size_cm: 30x20x8
voxels: 38x26x10
grid: F
grain: 0
collider: false
states:
  Tidy: cables parallel, a cable tie every ~6 cm
  Messy: tangled loops, no ties, one kinked cable
---

# CableBundle

## Prompt

### Shape
Build a voxel CableBundle for the Basement room, about 30x20x8 cm (38x26x10 voxels). Main forms: 4 cable colours, connectors.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: ties, labels, kinks.

## Parts (tier; cm size + colour key to fill before first build)

- [2] 4 cables 1.6 cm thick: cable_red, cable_yellow, cable_blue, cable_green
- [2] connectors 4x 3 x 1.6 x 1.6 cm, plastic_black + metal tip
- [3] cable ties 3x, plastic_white (Tidy) / 1x (Messy)
- [3] labels 2x 1.6 cm, label
- [3] kinks: sharp bends only in Messy
