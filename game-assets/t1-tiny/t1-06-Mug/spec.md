---
name: Mug
id: t1-06
tier: T1
room: Control
priority: V1
size_cm: 11x8x9
voxels: 14x10x12
grid: F
grain: 0
collider: false
states:
  Full: coffee surface (dark brown) just below the rim
  Empty: dark inner wall, no liquid, coffee stain ring at the bottom
---

# Mug

## Prompt

### Shape
Build a voxel Mug for the Control room, about 11x8x9 cm (14x10x12 voxels). Main forms: handle, rim.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: coffee surface, logo label, base ring, chip wear.

## Parts (tier; cm size + colour key to fill before first build)

- [2] handle: 3 x 2 x 7 cm, plastic_white
- [2] rim: 1 cm top ring, trim
- [3] coffee surface: 7 x 7 cm, wood_dark
- [3] logo label: 3 x 4 cm, warning_red
- [3] base ring: 6 x 6 cm, chassis_dark
- [3] chip wear: 1 cm, carved
