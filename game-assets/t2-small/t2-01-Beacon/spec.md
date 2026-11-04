---
name: Beacon
id: t2-01
tier: T2
room: Control + Basement
priority: V1
size_cm: 12x12x16
voxels: 15x15x20
grid: F
grain: 0
collider: false
states:
  Off: dark red dome
  On: bright red dome, lens ribs in a highlight key
---

# Beacon

## Prompt

### Shape
Build a voxel Beacon for the Control + Basement room, about 12x12x16 cm (15x15x20 voxels). Main forms: dome, base.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: lens ribs, bolts, wire, bulb.

## Parts (tier; cm size + colour key to fill before first build)

- [1] dome lens 8 x 8 x 10 cm stepped, warning_red (Off) / led_orange (On)
- [2] base 12 x 12 x 4 cm, chassis
- [2] top cap 3 x 3 x 1.5 cm, metal_dark
- [3] lens ribs 4x 0.8 cm vertical, seam
- [3] bolts 4x 1.6 cm, metal
- [3] wire 0.8 cm from base side, rubber
- [3] bulb 3 x 3 x 5 cm inside lens, led_yellow (On only)
