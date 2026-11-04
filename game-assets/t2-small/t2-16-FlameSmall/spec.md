---
name: FlameSmall
id: t2-16
tier: T2
room: Basement
priority: V1
size_cm: 30x30x50
voxels: 19x19x32
grid: S
grain: 0
collider: false
states:
  A: flame tips leaning left
  B: flame tips leaning right (same base and height ±2 voxels, for flicker)
---

# FlameSmall

## Prompt

### Shape
Build a voxel FlameSmall for the Basement room, about 30x30x50 cm (19x19x32 voxels). Main forms: 3 flame layers.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: hot core, stepped tips, ember voxels at the base.

## Parts (tier; cm size + colour key to fill before first build)

- [2] 3 flame layers (red / orange / yellow)
- [3] hot core
- [3] stepped tips
- [3] ember voxels at the base
