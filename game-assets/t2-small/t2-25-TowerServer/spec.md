---
name: TowerServer
id: t2-25
tier: T2
room: Basement
priority: UP
size_cm: 20x45x45
voxels: 26x58x58
grid: F
grain: 0
collider: false
states:
  On: LED `led_green`
  Hot: LED `led_orange`, darkened mesh vent
  Failed: LED `led_red`
  Off: LED `led_off`
---

# TowerServer

## Prompt

### Shape
Build a voxel TowerServer for the Basement room, about 20x45x45 cm (26x58x58 voxels). Main forms: case, front panel.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: mesh vent, drive bays, power button, status LED, network port, rear PSU.

## Parts (tier; cm size + colour key to fill before first build)

- [1] case 20 x 45 x 45 cm, chassis
- [2] front panel 20 x 1.6 x 45 cm, chassis_dark
- [3] status LED 2x2, led_green / led_orange / led_red / led_off
- [3] mesh vent (tile) 12 x 14 cm, seam (orange when Hot)
- [3] drive bays 2x 14 x 4.7 cm, metal_dark
- [3] power button 2.3 cm, metal
- [3] network port 3 x 2.3 cm on the rear, copper in plastic_black
- [3] rear PSU 14 x 1.6 x 12 cm, metal_dark with fan grille
