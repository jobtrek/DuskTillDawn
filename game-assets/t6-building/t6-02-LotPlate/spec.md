---
name: LotPlate
id: t6-02
tier: T6
room: Whole building
priority: V1
size_cm: 2000x1600x10
voxels: 1280x1024x6
grid: S
grain: 0.15
collider: true
---

# LotPlate

## Prompt

### Shape
Build a voxel LotPlate for the Whole building room, about 2000x1600x10 cm (1280x1024x6 voxels). Main forms: gravel, paving slabs, paint lines, drain covers, kerb. grain 0.15.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.15.

### Details
Add the small features: gravel, paving slabs, paint lines, drain covers, kerb. grain 0.15.

## Parts (tier; cm size + colour key to fill before first build)

- [2/3] gravel
- [2/3] paving slabs
- [2/3] paint lines
- [2/3] drain covers
- [2/3] kerb. grain 0.15

## Notes

- This is the asset itself (the base plate under the building), not a helper ground plane.
- Mostly flat: surface variety comes from `grain=0.15` and a few paint lines, drains and kerb voxels.
