---
name: Server1U
id: t2-19
tier: T2
room: Basement
priority: V1
size_cm: 45x60x4.4
voxels: 58x77x6
grid: F
grain: 0
collider: false
states:
  On: status LED `led_green`, drive-bay LEDs lit
  Hot: status LED `led_orange`, darkened vents
  Off: all LEDs in `led_off`
  Failed: status LED `led_red`, drive bay 2 ejected by 2 voxels
---

# Server1U

## Parts (tier; cm size + colour key to fill before first build)

- [1] chassis: 45 x 60 x 4.4 cm, chassis
- [2] bezel: 42 x 0.8 x 4.4 cm, chassis_dark
- [2] rack ear left: 1.6 x 5.5 x 4.4 cm, metal
- [2] rack ear right: 1.6 x 5.5 x 4.4 cm, metal
- [3] vents: 0.8 x 3 cm slits (8x), seam
- [3] drive bays: 4.7 x 3 cm (3x), metal_dark
- [3] activity LED: 1.6 cm, led_green
- [3] status LED: 1.6 cm, led_green
- [3] power button: 1.6 cm, trim
- [3] screws: 0.8 cm (4x), metal
- [3] top vents: 0.8 x 32 cm slots (8x), seam
- [3] label: 8 x 5 cm on top, label
