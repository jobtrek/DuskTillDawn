---
name: Room_Basement
id: t5-02
tier: T5
room: Basement
priority: V1
size_cm: 800x600x240
voxels: 512x384x154
grid: S
grain: 0
collider: true
pieces: Floor, Wall_Back, Wall_Front, Wall_Left, Wall_Right, Ceiling
wall_cm: 12
---

# Room_Basement

## Prompt

### Shape
Build a voxel Room Basement for the Basement room, about 800x600x240 cm (512x384x154 voxels). Main forms: floor drain, concrete seams, pipe brackets, conduit, wall stains.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: floor drain, concrete seams, pipe brackets, conduit, wall stains.

## Parts (tier; cm size + colour key to fill before first build)

- [2/3] floor drain 25 x 23 cm grate in a metal frame at the room centre-front, metal / metal_dark / seam slots
- [2/3] concrete seams 1 voxel grooves: floor slabs every 2 m, wall pour joints every 2 m + one form line at 1.3 m, seam
- [2/3] pipe brackets 6 x 6 x 25 cm on Wall_Back (5), Wall_Left (3), Wall_Right (3), metal_dark + metal arms
- [2/3] conduit 3 cm metal runs with clips + junction boxes on Wall_Back / Wall_Left / Wall_Right / Ceiling
- [2/3] wall stains damp patches 40-80 cm, stain / damp / rust streaks
- [2/3] stair door hole in `Room_Basement_Wall_Right` 100 x 205 cm (64 x 131 voxels), centre y = +2.1 m
- [2/3] painted lines 6 cm warning_yellow aisle line, drain ring, door approach lines, door-edge stripes
