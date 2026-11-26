# Where do I change X?

Paths from the repo root. `dtd/` = `dusk-till-dawn/`. Open these first, search only if they miss.

| To change… | Go to |
|---|---|
| Game state, SLA, Disk, points | `game-core/Night/GameState.cs` |
| Heat, battery, Traffic, UPS, Auto-shutdown, Eco Mode (numbers at the top) / Electrical fault chance, fault → Fire | `game-core/Power/ServerRoom.cs` / `game-core/Power/ElectricalFaultRisk.cs` |
| Clock, night/day hours, time speed | `game-core/Night/GameState.cs` (constants at top); debug speed: `scripts/run-fast.sh`; balancing log (`user://night_logs/*.csv`): `game-core/Night/NightLog.cs` |
| Incidents | `game-core/Incidents/Incident.cs`, one file per kind (`Fire.cs`, …); rules in `docs/player-flow/challenges.md`; Servers and Clients they hit: `game-core/Servers/` |
| Incident spawn rate / which incident spawns (random order) / per night | `game-core/Incidents/IncidentSpawner.cs` / `game-core/Incidents/IncidentPool.cs` / `game-core/Night/NightPlan.cs`; current night in `dtd/main/Main.cs` |
| Player name, points, held item | `game-core/Night/PlayerProfile.cs` |
| Points rule (fast bonus, failure penalty) | `game-core/Incidents/PointsRule.cs`; rule in `docs/player-flow/challenges.md` |
| Incident engine, failures, end of game | `game-core/Incidents/Game.cs`, `SoloGame.cs` |
| Weather kinds, chances, rain strength | `game-core/Weather/Weather.cs`, `WeatherSystem.cs` |
| Rain look, light dim, rain sound | `dtd/main/weather/Rain.cs`, `RainLook.cs` (Inspector values), `RainSound.cs`; sounds in `weather/sounds/` |
| Sun, moon, sky colours | `game-core/Weather/SunPosition.cs`, `MoonPosition.cs`; visuals in `dtd/main/Main.cs`, `dtd/main/sky/sky.gdshader` |
| Mail content (schedule, kinds) / inbox rules / Phishing mail → Fake e-mail Incident | `dtd/main/mail_ui/emails.json` / `game-core/Mails/Inbox.cs`, `Mail.cs` / `game-core/Mails/PhishingIncidents.cs`, `game-core/Incidents/PhishingEmail.cs` |
| Any UI: desk computer, Apps, HUD, Dashboard, UI motion | [where-to-change-ui.md](where-to-change-ui.md) |
| Server room rack, E key: replace disk, restart cooling, reboot servers | `dtd/main/disk_repair/DiskRepairSpot.cs`; prompt: `dtd/main/prompt_ui/` |
| Fire in the server room (flames, smoke) | `dtd/main/fire/FireEffect.cs`, `fire_effect.tscn`; flame positions: `Company/Flame*` in `main.tscn` |
| Pick up / put back the extinguisher (E, Q), put out the fire | `dtd/main/fire/ExtinguisherSpot.cs`, `extinguisher_spot.tscn`; model: `Company/Extinguisher` in `main.tscn`; held item: `game-core/Night/PlayerProfile.cs` |
| Where held items sit on screen | `Player/Camera3D/RightHand` in `dtd/main/main.tscn` |
| Player movement, camera | `dtd/player/Player.cs` |
| Scene tree, what's in the world | `dtd/main/main.tscn` (one owner per scene) |
| Basement / roof geometry, lights | `dtd/levels/basement/`, `dtd/levels/roof/` (`.blend` + scripts) |
| A 3D model (Element) | `game-assets/<tier>/<id>-<Name>/` → read [../assets/README.md](../assets/README.md) |
| Voxel SDK checks and build | `game-assets/sdk/src/voxel_sdk/` |
| CI | `.github/workflows/ci.yml` |
| Words and their meaning | `CONTEXT.md` |
| v1 numbers (rates, prices, nights) | `docs/game-v1.md` (human-owned) |

Not found here? Add the row after you find it.
