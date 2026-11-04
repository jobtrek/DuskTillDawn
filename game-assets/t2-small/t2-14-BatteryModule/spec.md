---
name: BatteryModule
id: t2-14
tier: T2
room: Basement
priority: V1
size_cm: 44x45x13
voxels: 56x58x17
grid: F
grain: 0
collider: false
states:
  Charged: LED bar fully green
  Low: LED bar one red segment
  Overheating: red LED, orange discolouration on the case, darkened vents, 1-voxel bulge on the side
---

# BatteryModule

## Prompt

### Shape
Build a voxel BatteryModule for the Basement room, about 44x45x13 cm (56x58x17 voxels). Main forms: case, handles.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: terminals, BMS display, LED bar, vents, label.

## Parts (tier; cm size + colour key to fill before first build)

- [1] case 44 x 44 x 11 cm, chassis
- [2] handles 2x 5.5 x 0.8 x 3 cm on the front, metal
- [3] terminals 2x 3 x 3 x 1.6 cm on top, copper + warning_red / rubber caps
- [3] BMS display 12 x 0.8 x 3 cm, screen + coloured digits
- [3] LED bar 5x 2x2 voxels, led_green / led_red / led_orange / led_off
- [3] vents 4 slots each side, seam (orange when Overheating)
- [3] label 15 x 12 cm on top, label
