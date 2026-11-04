---
name: RackFrame
id: t3-13
tier: T3
room: Basement
priority: V1
size_cm: 60x80x120
voxels: 77x102x154
grid: F
grain: 0
collider: true
states:
  Ok: top LED `led_green`
  Warn: top LED `led_orange`
  Failed: top LED `led_red`
---

# RackFrame

## Prompt

### Shape
Build a voxel RackFrame for the Basement room, about 60x80x120 cm (77x102x154 voxels). Main forms: posts, side panels, door.

### Materials
Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.

### Details
Add the small features: rail holes, vent slots, casters, lock, label.

## Parts (tier; cm size + colour key to fill before first build)

- [1] posts 4x 2.3 x 2.3 x 113 cm, metal
- [2] side panels 2x 1.6 x 78 x 112 cm, chassis
- [2] front door frame + mesh bars 54 x 1.6 x 109 cm, chassis_dark
- [3] rail holes (tile) 24U, 1 x 2 voxels every 6 voxels on the front posts, seam
- [3] vent slots 8x on each side panel, seam
- [3] casters 4x, rubber on metal_dark fork
- [3] lock + handle 1.6 x 3 cm on the door, metal
- [3] top-of-rack LED 4x4: led_green (Ok) / led_orange (Warn) / led_red (Failed)
- [3] label (24U) 18 x 4 cm, label
