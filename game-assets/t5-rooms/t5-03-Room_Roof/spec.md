---
name: Room_Roof
id: t5-03
tier: T5
room: Roof
priority: V1
size_cm: 1000x800x100
voxels: 640x512x64
grid: S
grain: 0
collider: true
pieces: Floor, Parapet_Back, Parapet_Front, Parapet_Left, Parapet_Right, Chimney
wall_cm: 12
---

## Prompt
Voxel roof room, 1000x800x100 cm (640x512x64 voxels), hard-edged, grain 0, colour keys from `game-assets/palette.py`. Floor z0-9, parapets z10-63, centred x/y.
## Parts
- [1] floor slab: 1000 x 800 x 16 cm, chassis_dark
- [1] parapets: 12 cm thick, up to 100 cm, concrete
- [2] coping: 12 x 5 cm cap on parapets, trim
- [2] drain outlets: 2 x 12 x 12 cm scuppers in front parapet, metal_dark
- [2] gutter: 10 x 6 cm channel along front base, metal
- [3] tar seams: 1 voxel lines every 100 cm, seam
- [3] hatch hole: 80 x 80 cm carved, concrete curb 20 cm high
- [3] vents: 3 pipe vents 6 x 6 x 30 cm, metal + metal_dark cap
- [2] chimney: 50 x 50 x 86 cm at back right, solar array fits front-centre, rust + concrete cap
