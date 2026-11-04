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
  On: status LED `led_green`, drive-bay LEDs lit
  Hot: status LED `led_orange`, darkened vents
  Off: all LEDs in `led_off`
  Failed: status LED `led_red`, drive bay 2 ejected by 2 voxels
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

- [1] chassis 45 x 60 x 4.4 cm, chassis
- [2] bezel 42 x 0.8 x 4.4 cm, chassis_dark
- [2] rack ear handles 2x 1.6 x 5.5 x 4.4 cm, metal
- [3] vents (tile) 8 slits 0.8 x 3 cm, seam
- [3] drive bays 3x 4.7 x 3 cm, metal_dark
- [3] 2x2 LEDs: activity (led_green) + status (green / orange / red / off)
- [3] power button 1.6 cm, trim; screws 4x, metal
- [3] label 8 x 5 cm on top, label
