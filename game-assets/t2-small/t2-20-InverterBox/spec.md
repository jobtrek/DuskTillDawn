---
name: InverterBox
id: t2-20
tier: T2
room: Roof
priority: V1
size_cm: 40x20x60
voxels: 51x26x77
grid: F
grain: 0
collider: false
states:
  On: display lit, green LED
  Off: display dark, all LEDs off
  Broken: cracked display, red LED, scorch marks around the vents
---

# InverterBox

## Prompt

### Shape
Build a voxel InverterBox for the Roof room, about 40x20x60 cm (51x26x77 voxels). Main forms: housing, display.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: LEDs, heat fins, DC switch, cable glands, label.

## Parts (tier; cm size + colour key to fill before first build)

- [2] housing
- [2] display
- [3] LEDs
- [3] heat fins
- [3] DC switch
- [3] cable glands
- [3] label
