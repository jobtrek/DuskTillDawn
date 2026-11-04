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

- [2] casing 10 x 15 x 2.3 cm, metal
- [2] label 7 x 7 x 0.1 cm, label
- [3] screws 4x 0.8 cm, metal_dark
- [3] connector + pins 8 x 1.6 x 1.6 cm, plastic_black / copper
- [3] PCB edge 10 x 15 x 0.8 cm, pcb
- [3] 2x2 LED (green Healthy / red Failed)
