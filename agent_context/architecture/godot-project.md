# Godot project (`dusk-till-dawn/`)

Godot 4.7.2 .NET, C#, `net8.0`, nullable on. Main scene: `main/main.tscn`.
References `game-core` through `DuskTillDawn.csproj`.
Autoload: `MCPGameBridge` (local dev tooling for the Godot MCP; its addon
`addons/godot_mcp/` is git-ignored, not game code).

## Runtime flow

```
Main (Node3D, main/Main.cs)       owns the one GameState and SoloGame, calls Tick() each frame,
 │                                ticks a ChallengeSpawner (a challenge every 20 s),
 │                                drives sun/moon/sky from SunPosition/MoonPosition
 ├─ Hud (hud_ui/)                 reads GameState: clock, SLA bar, points, banner
 ├─ Player (player/Player.cs)     CharacterBody3D + camera
 ├─ Basement_Levels               imported .blend, Element States as child nodes
 └─ desk computer                 DetectPlayerNearComputer shows the menu when near
     └─ ComputerMenu (computer_ui/)   emits MailPressed / MonitoringPressed / HelpPressed / AdminPressed
         ├─ MailController   → MailApp     (mail_ui/)
         ├─ MonitoringController → MonitorApp (monitor_ui/)
         └─ AdminController  → AdminApp    (admin_ui/)  ActionPressed → Main repairs challenges
```

## Pattern: App + Controller

- `*App.cs` (Control): view only. Shows values, emits signals. No game logic.
- `*Controller.cs` (Node): glue. Gets `GameState` from `Main.State`, listens to the
  menu's signals, pushes values into the App, calls `GameState` methods on actions.
- Only one app window is open at a time; walking away closes them.
## Folders

| Folder | Holds |
|---|---|
| `main/` | `Main.cs`, `main.tscn`, sky shader, UI folders `*_ui/` |
| `main/computer_ui/` | Desk OS menu, `BlockReveal`, pixel textures |
| `player/` | Player body and camera |
| `levels/<floor>/` | Floor `.blend` files + per-level scripts |
| `assets/` | Building `.blend` files |

UI style rules: [../gui/README.md](../gui/README.md).
