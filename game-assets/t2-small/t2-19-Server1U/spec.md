---
name: Server1U
id: t2-19
tier: T2
room: Basement
priority: V1
size_cm: 45x60x4.4
voxels: 58x77x6
grid: F
grain: 0
collider: false
states:
  On: all LEDs green, drive-bay LEDs lit
  Off: all LEDs in `led_off`
  Failed: red LED, drive bay 2 ejected by 2 voxels
---

# Server1U

## Prompt

### Shape
Build a voxel Server1U for the Basement room, about 45x60x4.4 cm (58x77x6 voxels). Main forms: bezel, handles.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: vents, drive bays, 2×2 LEDs, power button, screws.

## Parts (tier; cm size + colour key to fill before first build)

- [2] bezel
- [2] handles
- [3] vents (tile)
- [3] drive bays
- [3] 2×2 LEDs
- [3] power button
- [3] screws
- [3] label
