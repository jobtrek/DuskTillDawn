# Coexist with the other UI

Part of the [GUI guide](README.md).

Every new or changed component shares the screen with the UI that already exists (find it: `grep -rl 'CanvasLayer\|type="Control"' --include='*.tscn' dusk-till-dawn/`). Before placing it, list each component that can be visible at the same time, then make sure:

- **Space**: its anchored rect does not cover another visible component's rect (HUD corners, prompts, open panels), at 16:9 and at a narrow window.
- **Stacking**: when it must sit on top (a dialog over the HUD), the draw order is deliberate: a higher `CanvasLayer.layer`, or the HUD hides while the panel is open.
- **Input**: one owner at a time. A full panel that frees the mouse (`Input.MouseMode`) or eats keys releases them on close, and two panels never claim the same key or the mouse at once.
- **Meaning**: it does not contradict another component: same action, same key and label everywhere; no two prompts or counters showing different values for the same state; it opens only in a state the other UI agrees with (no "press E" prompt while a panel already covers the screen).
