# Where do I change X?

Paths from the repo root. `dtd/` = `dusk-till-dawn/`. Open these first, search only if they miss.

| To change… | Go to |
|---|---|
| Game state, SLA, Heat/Power/Traffic/Disk, points | `game-core/GameState.cs` |
| Clock, night/day hours, time speed | `game-core/GameState.cs` (constants at top); debug speed: `scripts/run-fast.sh` |
| Incidents (code name Challenge) | `game-core/Challenge.cs`, one file per challenge (`Fire.cs`, …); rules in `docs/player-flow/challenges.md`; Servers and Clients they hit: `game-core/Servers/` |
| Game state, SLA, Disk, points | `game-core/GameState.cs` |
| Heat, battery, Traffic, UPS, Auto-shutdown, Eco Mode (numbers at the top) / Electrical fault chance, fault → Fire | `game-core/ServerRoom.cs` / `game-core/ElectricalFaultRisk.cs` |
| Clock, night/day hours, time speed | `game-core/GameState.cs` (constants at top); debug speed: `scripts/run-fast.sh`; balancing log (`user://night_logs/*.csv`): `game-core/NightLog.cs` |
| Incidents (code name Challenge) | `game-core/Challenge.cs`, one file per challenge (`Fire.cs`, …); rules in `docs/player-flow/challenges.md` |
| Incident spawn rate / which incident spawns (random order) | `game-core/ChallengeSpawner.cs` / `game-core/StoreInstancesOfIncidents.cs`; list of incidents in `dtd/main/Main.cs` |
| Player name, points, room | `game-core/PlayerProfile.cs` |
| Points rule (fast bonus, failure penalty) | `game-core/PointsRule.cs`; rule in `docs/player-flow/challenges.md` |
| Game modes (solo, duo), end of game, winner | `game-core/Game.cs`, `SoloGame.cs`, `DuoGame.cs` |
| Weather kinds, chances, rain strength | `game-core/Weather.cs`, `WeatherSystem.cs` |
| Rain look, light dim, rain sound | `dtd/main/weather/Rain.cs`, `RainLook.cs` (Inspector values), `RainSound.cs`; sounds in `weather/sounds/` |
| Sun, moon, sky colours | `game-core/SunPosition.cs`, `MoonPosition.cs`; visuals in `dtd/main/Main.cs`, `dtd/main/sky/sky.gdshader` |
| Mail content (schedule, kinds) / inbox rules | `dtd/main/mail_ui/emails.json` / `game-core/Inbox.cs`, `Mail.cs` |
| Mail window (view) / its logic | `dtd/main/mail_ui/MailApp.cs` / `MailController.cs` |
| Monitoring window / its logic | `dtd/main/monitor_ui/MonitorApp.cs` / `MonitoringController.cs` |
| Admin window (traffic filter, Power: faulty circuit, Eco Mode, Graceful shutdown) / what each does | `dtd/main/admin_ui/AdminApp.cs` / `AdminController.cs` |
| Desk computer menu, app icons, signals; block-pop open animation | `dtd/main/computer_ui/ComputerMenu.cs`, `computer_menu.tscn`; `BlockReveal.cs` |
| When the computer opens (player nearby) | `dtd/main/DetectPlayerNearComputer.cs` |
| Server room rack, E key: replace disk, restart cooling, reboot servers | `dtd/main/disk_repair/DiskRepairSpot.cs`; prompt: `dtd/main/prompt_ui/` |
| Fire in the server room (flames, smoke) | `dtd/main/fire/FireEffect.cs`, `fire_effect.tscn`; flame positions: `Company/Flame*` in `main.tscn` |
| Pick up / put back the extinguisher (E, Q), put out the fire | `dtd/main/fire/ExtinguisherSpot.cs`, `extinguisher_spot.tscn`; model: `Company/Extinguisher` in `main.tscn`; held item: `game-core/PlayerProfile.cs` |
| Where held items sit on screen | `Player/Camera3D/RightHand` in `dtd/main/main.tscn` |
| Dashboard (HUD panel: points, failures, running challenges) | `dtd/main/dashboard_ui/Dashboard.cs`, `dashboard.tscn` |
| HUD: clock, SLA bar, points, banner / night report at 08:00 | `dtd/main/hud_ui/Hud.cs`, `SlaBar.cs`, `hud.tscn` / `dtd/main/night_report_ui/`, `game-core/NightReport.cs`, when: `Main.cs` |
| UI pixel textures | `scripts/gen_computer_ui_textures.py` → `dtd/main/computer_ui/textures/` (HUD: `dtd/main/hud_ui/textures/`) |
| Player movement, camera | `dtd/player/Player.cs` |
| Scene tree, what's in the world | `dtd/main/main.tscn` (one owner per scene) |
| Basement / roof geometry, lights | `dtd/levels/basement/`, `dtd/levels/roof/` (`.blend` + scripts) |
| A 3D model (Element) | `game-assets/<tier>/<id>-<Name>/` → read [../assets/README.md](../assets/README.md) |
| Voxel SDK checks and build | `game-assets/sdk/src/voxel_sdk/` |
| CI | `.github/workflows/ci.yml` |
| Words and their meaning | `CONTEXT.md` |
| v1 numbers (rates, prices, nights) | `docs/game-v1.md` (human-owned) |

Not found here? Add the row after you find it.
