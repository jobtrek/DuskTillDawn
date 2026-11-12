---
name: TransferSwitchCabinet
id: t4-20
tier: T4
room: Generator
priority: V1
size_cm: 87.5x37.5x181.25
voxels: 56x24x116
grid: S
grain: 0
collider: true
states:
  Grid: power comes from the grid
  Solar: power comes from the solar panels
  Battery: power comes from the battery bank
  Generator: power comes from the generator
---

# TransferSwitchCabinet

Cabinet that chooses where the building's power comes from: solar, battery, grid or generator. The lit lamp and the selector pointer show the active source. Front at -y. Base State is Grid.

## Parts (tier; cm size + colour key)

- [1] plinth: 87.5 x 37.5 x 12.5 cm, chassis_dark
- [1] cabinet body: 81.25 x 31.25 x 162.5 cm, chassis
- [1] cabinet roof: 87.5 x 37.5 x 6.25 cm, chassis_dark
- [2] door panel: 68.75 x 3.12 x 143.75 cm, metal
- [2] status screen: 31.25 x 3.12 x 15.62 cm, screen
- [2] selector base: 25 x 3.12 x 25 cm, trim
- [2] selector knob: 15.62 x 6.25 x 15.62 cm, metal_dark
- [2] solar label: 25 x 3.12 x 9.38 cm, label
- [2] solar lamp: 9.38 x 3.12 x 9.38 cm, led_off
- [2] battery label: 25 x 3.12 x 9.38 cm, label
- [2] battery lamp: 9.38 x 3.12 x 9.38 cm, led_off
- [2] grid label: 25 x 3.12 x 9.38 cm, label
- [2] grid lamp: 9.38 x 3.12 x 9.38 cm, led_orange
- [2] generator label: 25 x 3.12 x 9.38 cm, label
- [2] generator lamp: 9.38 x 3.12 x 9.38 cm, led_off
- [3] solar mark: 12.5 x 3.12 x 3.12 cm, label_text
- [3] battery mark: 15.62 x 3.12 x 3.12 cm, label_text
- [3] grid mark: 9.38 x 3.12 x 3.12 cm, label_text
- [3] generator mark: 18.75 x 3.12 x 3.12 cm, label_text
- [3] selector pointer: 3.12 x 3.12 x 7.81 cm, warning_red
- [3] door handle: 6.25 x 3.12 x 25 cm, trim
- [3] hinges: 4.69 x 3.12 x 12.5 cm, 3 copies, metal_dark
- [3] warning sticker: 15.62 x 3.12 x 15.62 cm, warning_yellow
