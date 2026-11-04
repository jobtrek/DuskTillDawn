---
name: StairShaft
id: t5-04
tier: T5
room: Between floors
priority: V1
size_cm: 150x350x280
voxels: 96x224x179
grid: S
grain: 0
collider: true
---

# StairShaft

## Prompt

### Shape
Build a voxel StairShaft for the Between floors room, about 150x350x280 cm (96x224x179 voxels). Main forms: walls, door holes, light fixture mounts, wall trim.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: walls, door holes, light fixture mounts, wall trim.

## Parts (tier; cm size + colour key to fill before first build)

- [2/3] walls
- [2/3] door holes (into each floor's `Wall_Right`)
- [2/3] light fixture mounts
- [2/3] wall trim

## Notes

- One object, reused for each floor.
- Sits outside the shared 800 × 600 footprint, on the +x side of each floor.
