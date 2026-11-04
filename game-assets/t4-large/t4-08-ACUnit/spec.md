---
name: ACUnit
id: t4-08
tier: T4
room: Basement
priority: V1
size_cm: 70x90x200
voxels: 45x58x128
grid: S
grain: 0
collider: true
states:
  On: display lit, louvres open, green LED
  Off: display dark, louvres closed
  Broken: red LED, one door hanging open, dent in the cabinet side
---

# ACUnit

## Prompt

### Shape
Build a voxel ACUnit for the Basement room, about 70x90x200 cm (45x58x128 voxels). Main forms: cabinet, doors.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: display, louvres, handles, logo, LED, pipe stubs, feet.

## Parts (tier; cm size + colour key to fill before first build)

- [1] cabinet 70 x 90 x 200 cm, plastic_white
- [2] lower double doors 63 x 1.6 x 117 cm, paint_wall (_Broken: left door buckled out)
- [2] upper control panel 63 x 1.6 x 70 cm, chassis
- [3] display 30 x 17 cm, screen + digits led_green (On) / led_red (Broken)
- [3] louvres 6 slots, seam; handles 2x metal
- [3] logo bar label; LED 2x2 led_green / led_off / led_red
- [3] pipe stubs 2x 5 cm on the back, copper; feet 4x metal_dark
