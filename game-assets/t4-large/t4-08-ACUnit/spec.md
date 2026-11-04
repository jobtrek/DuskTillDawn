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

- [2] cabinet, doors
- [3] display, louvres, handles, logo, LED, pipe stubs, feet

(A 2nd AC upgrade reuses this model.)
