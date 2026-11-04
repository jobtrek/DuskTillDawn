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

- [2] flame outer layer 30 x 30 x 50 cm stepped, led_red
- [2] flame mid layer, led_orange
- [2] flame inner layer, led_yellow
- [3] hot core 3 x 3 x 9 cm, plastic_white
- [3] stepped tips, 1 voxel wide (_A leans left, _B leans right)
- [3] ember voxels 4x at the base, led_yellow
