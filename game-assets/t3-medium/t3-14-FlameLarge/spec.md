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

- [2] flame outer layer 80 x 80 x 120 cm stepped, led_red
- [2] flame mid layer, led_orange
- [2] flame inner layer, led_yellow
- [3] hot core 7 x 7 x 20 cm, plastic_white
- [3] stepped tips + side licks (_A leans left, _B leans right)
- [3] ember voxels 8x around the base, led_yellow
