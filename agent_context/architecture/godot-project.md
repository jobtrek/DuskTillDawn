# Godot project (`dusk-till-dawn/`)

Godot 4.7.2 .NET, C#, `net8.0`, nullable on. Main scene: `main/main.tscn`; uses `game-core` via `DuskTillDawn.csproj`.
Autoload: `MCPGameBridge` (dev tooling for the Godot MCP, configured in root `.mcp.json`; set
`GODOT_PATH` if Godot isn't at the default path. Addon `addons/godot_mcp/` is git-ignored).

## Runtime flow

```
Main (Node3D, main/Main.cs)       owns the one GameState and SoloGame, calls Tick() each frame,
 │                                ticks a ChallengeSpawner built from NightPlan (night 1: every 180 s),
 │                                drives sun/moon/sky, passes Weather to Rain/RainSound/Lightning, adds the
 │                                NightReportPanel (night_report_ui/) shown at 08:00 or on a loss
 ├─ Hud (hud_ui/)                 reads GameState: clock, SLA bar, points, banner
 ├─ Dashboard (dashboard_ui/)     reads SoloGame: points, failures, running challenges
 ├─ Player (player/Player.cs)     CharacterBody3D + camera
 ├─ Basement_Levels               imported .blend, Element States as child nodes
 ├─ DiskRepairSpot (disk_repair/) server room rack: red disk while a BrokenDisk runs, E repairs it (not while Busy)
 ├─ PanelRepairSpot (solar_repair/) Company roof: LightningBolt strikes a SolarPanel, each broken one smokes (PanelDamage) and has its own E zone (not while Busy)
 ├─ FireEffect (fire/)           server room: flickering flames + smoke while a Fire runs
 ├─ ExtinguisherSpot (fire/)     server room wall: E puts the Fire out (not while Busy)
 └─ desk computer                 DetectPlayerNearComputer shows the menu when near
     └─ ComputerMenu (computer_ui/)   emits MailPressed / MonitoringPressed / HelpPressed / AdminPressed
         ├─ MailController   → MailApp     (mail_ui/)  ReportPressed → Main.Phishing.Report
         ├─ MonitoringController → MonitorApp (monitor_ui/)
         └─ AdminController  → AdminApp    (admin_ui/)  ActionPressed → Game.Repair, shows the Busy countdown
```

## Pattern: App + Controller

- `*App.cs` (Control): view only. Shows values, emits signals. No game logic.
- `*Controller.cs` (Node): glue. Gets `GameState` from `Main.State`, listens to the
  menu's signals, pushes values into the App, calls `GameState` methods on actions.
- Only one app window is open at a time; walking away closes them.

Folders and what they hold: [godot-folders.md](godot-folders.md).
