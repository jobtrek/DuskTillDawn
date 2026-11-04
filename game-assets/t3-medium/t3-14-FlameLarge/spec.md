---
name: FlameLarge
id: t3-14
tier: T3
room: Basement
priority: V1
size_cm: 80x80x120
voxels: 51x51x77
grid: S
grain: 0
collider: false
states:
  A: flame tips leaning left
  B: flame tips leaning right (same base and height ±4 voxels, for flicker)
---

# FlameLarge

## Prompt

### Shape
Build a voxel FlameLarge for the Basement room, about 80x80x120 cm (51x51x77 voxels). Main forms: 3 flame layers.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: hot core, stepped tips, ember voxels at the base.

## Parts (tier; cm size + colour key to fill before first build)

- [2] 3 flame layers (red / orange / yellow)
- [3] hot core
- [3] stepped tips
- [3] ember voxels at the base
