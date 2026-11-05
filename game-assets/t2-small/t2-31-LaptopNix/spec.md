---
name: LaptopNix
id: t2-31
tier: T2
room: Basement
priority: UP
size_cm: 33x23x3.1
voxels: 42x29x4
grid: F
grain: 0
collider: false
states:
  On: LED `led_green`
  Hot: LED `led_orange`, darkened vents
  Failed: LED `led_red`
  Off: LED `led_off`
  HalfOpen: lid hinged open 45 degrees, keyboard and screen visible, LED `led_green`
  FullOpen: lid hinged open 110 degrees, leaning back, LED `led_green`
---

# LaptopNix

ThinkPad-style black laptop with a NixOS snowflake logo on the lid. Detail: wordmark strip with a red dot, USB, HDMI and jack ports, full keyboard with a red TrackPoint and buttons, trackpad, fingerprint reader, palm-rest stickers, camera, terminal on the screen when open. Night den prop.

## Parts (tier; cm size + colour key to fill before first build)

- [1] base: 33 x 22 x 1.6 cm, chassis_dark
- [1] lid: 33 x 22 x 0.8 cm, chassis
- [2] hinges: 4.7 x 0.8 x 2.3 cm, 2 copies, metal_dark
- [3] status LED: 1.6 x 0.8 x 1.6 cm, led_green
- [3] vents: 0.8 x 0.8 x 0.8 cm, 6 copies, seam
- [3] rubber feet: 2.3 x 1.6 x 0.8 cm, 4 copies, rubber
- [3] power port: 0.8 x 3.1 x 0.8 cm, plastic_black
