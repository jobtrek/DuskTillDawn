# Godot project folders (`dusk-till-dawn/`)

Runtime flow and the App + Controller pattern: [godot-project.md](godot-project.md).

| Folder | Holds |
|---|---|
| `main/` | `Main.cs`, `main.tscn`, sky shader, `weather/` (rain nodes), UI folders `*_ui/`, shared UI helpers (`UiMotion.cs`, `BusyText.cs`, `action_button.tres`) |
| `main/rack_choice_ui/` | Server room rack choice popup (`RackChoice`) |
| `main/computer_ui/` | Desk OS menu, `BlockReveal`, pixel textures |
| `player/` | Player body and camera |
| `levels/<floor>/` | Floor `.blend` files + per-level scripts |
| `assets/` | Building `.blend` files |

UI style rules: [../gui/README.md](../gui/README.md).
