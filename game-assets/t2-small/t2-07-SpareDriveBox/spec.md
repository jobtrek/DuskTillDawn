---
name: SpareDriveBox
id: t2-07
tier: T2
room: Basement
priority: V1
size_cm: 20x30x10
voxels: 26x38x13
grid: F
grain: 0
collider: false
states:
  Full: lid open, 4 drives sitting in the foam slots
  Empty: lid open, foam slots empty
---

# SpareDriveBox

## Prompt

### Shape
Build a voxel SpareDriveBox for the Basement room, about 20x30x10 cm (26x38x13 voxels). Main forms: box, lid.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: hinge, latch, foam, labels, drives inside.

## Parts (tier; cm size + colour key to fill before first build)

- [1] box 20 x 30 x 5.5 cm, chassis
- [2] lid 20 x 30 x 1.6 cm, ajar (open ~4 cm at the front), trim
- [3] hinge 2x 1.6 x 1.6 x 1.6 cm at the back, metal
- [3] latch 3 x 0.8 x 2.3 cm at the front, metal
- [3] foam 18.8 x 28 x 5 cm, fabric_dark
- [3] labels 2x 4 x 3 cm on lid / drive, label
- [3] drives inside: 2x 15 x 10 x 3 cm, metal (Full only), recess chassis_dark (Empty)
