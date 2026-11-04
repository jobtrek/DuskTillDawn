---
name: SolarPanel
id: t4-02
tier: T4
room: Roof
priority: V1
size_cm: 100x4x165
voxels: 128x5x211
grid: F
grain: 0
collider: false
states:
  Clean: evenly blue cells, bright glass
  Dirty: dust patches in a pale brown key, a few leaf voxels, duller glass
  Damaged: diagonal crack lines across 3–4 cells, one missing cell corner, frame bent by 1–2 voxels
---

# SolarPanel

## Prompt

### Shape
Build a voxel SolarPanel for the Roof room, about 100x4x165 cm (128x5x211 voxels). Main forms: frame, glass.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: 60 cells, busbars, junction box, corner keys, label.

## Parts (tier; cm size + colour key to fill before first build)

- [2] frame
- [2] glass
- [3] 60 cells (tile)
- [3] busbars
- [3] junction box
- [3] corner keys
- [3] label
