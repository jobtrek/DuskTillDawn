---
name: HardDrive
id: t1-07
tier: T1
room: Basement
priority: V1
size_cm: 10x15x3
voxels: 13x19x4
grid: F
grain: 0
collider: false
states:
  Healthy: green LED, clean label
  Failed: red LED, scuffed label, one scorched corner
---

# HardDrive

## Prompt

### Shape
Build a voxel HardDrive for the Basement room, about 10x15x3 cm (13x19x4 voxels). Main forms: casing, label.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: screws, connector pins, PCB edge, 2×2 LED.

## Parts (tier; cm size + colour key to fill before first build)

- [2] casing
- [2] label
- [3] screws
- [3] connector pins
- [3] PCB edge
- [3] 2×2 LED (green / red)
