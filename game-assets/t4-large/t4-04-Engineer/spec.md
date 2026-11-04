---
name: Engineer
id: t4-04
tier: T4
room: All
priority: V1
size_cm: 50x30x172
voxels: 32x19x110
grid: S
grain: 0.08
collider: false
pieces:
  Head: head, hair and face (eyes, nose, mouth, ears)
  Torso: hoodie, lanyard badge, belt, pockets
  Arm: sleeve, cuff, hand with fingers as 2-voxel blocks (build the right arm; mirror in Blender)
  Leg: trousers, shoe with sole (build the right leg; mirror in Blender)
---

# Engineer

## Prompt

### Shape
Build a voxel Engineer for the All room, about 50x30x172 cm (32x19x110 voxels). Main forms: head, hoodie.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.08.

### Details
Add the small features: face, hair, pockets, lanyard badge, belt, shoes, hands.

## Parts (tier; cm size + colour key to fill before first build)

- [2] head (blob ok, organic), hoodie
- [3] face, hair, pockets, lanyard badge, belt, shoes, hands

## Notes

- Pieces are built in place in the full-body frame (feet z=0). Rig by hand in Blender.
