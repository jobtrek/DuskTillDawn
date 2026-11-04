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

- [2] case
- [2] handles
- [3] terminals
- [3] BMS display
- [3] LED bar
- [3] vents
- [3] label
