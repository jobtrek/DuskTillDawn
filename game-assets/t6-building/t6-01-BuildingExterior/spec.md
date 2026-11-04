---
name: BuildingExterior
id: t6-01
tier: T6
room: Whole building
priority: V1
size_cm: 1000x700x400
voxels: 640x448x256
grid: S
grain: 0
collider: true
---

# BuildingExterior

## Prompt

### Shape
Build a voxel BuildingExterior for the Whole building room, about 1000x700x400 cm (640x448x256 voxels). Main forms: windows, entrance door, wall panels, downpipes, outdoor AC.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: windows, entrance door, wall panels, downpipes, outdoor AC.

## Parts (tier; cm size + colour key to fill before first build)

- [2/3] windows (tile)
- [2/3] entrance door
- [2/3] wall panels
- [2/3] downpipes
- [2/3] outdoor AC
- [2/3] roof edge (matches the `Room_Roof` parapets)
- [2/3] signage

## Notes

- Includes the StairShaft on the +x side. Shown in the menu and night report only. Hollow shell only (`hollow`), no interior. Keep it under 40 000 faces: windows with `tile`, no per-voxel grain.
