---
name: PDU
id: t2-26
tier: T2
room: Basement
priority: UP
size_cm: 44x10x4.4
voxels: 56x13x6
grid: F
grain: 0
collider: false
states:
  On: status LED `led_green`
  Overload: status LED `led_orange`, load bar full
  Tripped: status LED `led_red`, breaker switch down, outlet LEDs `led_off`
  Off: status LED `led_off`
---

# PDU

## Parts (tier; cm size + colour key to fill before first build)

- [1] case: 40.6 x 8.6 x 4.7 cm, chassis
- [2] ear left: 1.6 x 5.5 x 3.9 cm, metal
- [2] ear right: 1.6 x 5.5 x 3.9 cm, metal
- [3] outlets: 3.1 x 0.8 x 2.3 cm (8x), plastic_black
- [3] outlet pins: 1.6 x 0.8 x 0.8 cm (8x), metal
- [3] breaker switch: 3.1 x 0.8 x 3.1 cm, plastic_black
- [3] breaker lever: 1.6 x 0.8 x 1.6 cm (up On, down red Tripped), plastic_white
- [3] status LED: 1.6 x 0.8 x 1.6 cm (colour per State), led_green
- [3] power cord: 3.1 x 1.6 x 3.1 cm (back), rubber
- [3] label: 7.8 x 3.9 x 0.8 cm (on top), label
