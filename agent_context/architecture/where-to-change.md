# Where do I change X?

Paths from the repo root. `dtd/` = `dusk-till-dawn/`. Open these first, search only if they miss.

| To change… | Go to |
|---|---|
| Game state, SLA, Disk, points | `game-core/GameState.cs` |
| Heat, battery, Traffic, UPS, Auto-shutdown, Eco Mode (numbers at the top) / Electrical fault chance, fault → Fire | `game-core/ServerRoom.cs` / `game-core/ElectricalFaultRisk.cs` |
| Clock, night/day hours, time speed | `game-core/GameState.cs` (constants at top); debug speed: `scripts/run-fast.sh`; balancing log (`user://night_logs/*.csv`): `game-core/NightLog.cs` |
| Incidents (code name Challenge) | `game-core/Challenge.cs`, one file per challenge (`Fire.cs`, …); rules in `docs/player-flow/challenges.md`; Servers and Clients they hit: `game-core/Servers/` |
| Incident spawn rate / which incident spawns (random order) / per night | `game-core/ChallengeSpawner.cs` / `game-core/StoreInstancesOfIncidents.cs` / `game-core/NightPlan.cs`; current night in `dtd/main/Main.cs` |
| Demo run: 3 Incidents easy → hard (Fire, traffic attack, Phishing mail), order, time limits | `game-core/ChallengeLadder.cs` (`Demo`) |
| Player name, points, room | `game-core/PlayerProfile.cs` |
| Points rule (fast bonus, failure penalty) | `game-core/PointsRule.cs`; rule in `docs/player-flow/challenges.md` |
| Game modes (solo, duo), end of game, winner | `game-core/Game.cs`, `SoloGame.cs`, `DuoGame.cs` |
| Weather kinds, chances, rain strength, strike rate | `game-core/Weather.cs`, `WeatherSystem.cs`; strike hits a panel: `LightningDamage.cs` |
| Rain look, light dim, rain sound, lightning flash + thunder | `dtd/main/weather/Rain.cs`, `RainLook.cs` (Inspector values), `RainSound.cs`, `Lightning.cs`; sounds in `weather/sounds/` (thunder clips: `thunder_sounds.tres`; sources: `CREDITS.md`) |
| Sun, moon, sky colours | `game-core/SunPosition.cs`, `MoonPosition.cs`; visuals in `dtd/main/Main.cs`, `dtd/main/sky/sky.gdshader` |
| Mail content (schedule, kinds) / inbox rules / Phishing mail → Fake e-mail challenge | `dtd/main/mail_ui/emails.json` / `game-core/Inbox.cs`, `Mail.cs` / `game-core/PhishingIncidents.cs`, `PhishingEmail.cs` |
| Any UI: desk computer, Apps, HUD, Dashboard, UI motion | [where-to-change-ui.md](where-to-change-ui.md) |
| Server room rack, E key: replace disk, restart cooling, reboot servers | `dtd/main/disk_repair/DiskRepairSpot.cs`; prompt: `dtd/main/prompt_ui/` |
| Fire in the server room (flames, smoke) | `dtd/main/fire/FireEffect.cs`, `fire_effect.tscn`; flame positions: `Company/Flame*` in `main.tscn` |
| Pick up / put back the extinguisher (E, Q), put out the fire | `dtd/main/fire/ExtinguisherSpot.cs`, `extinguisher_spot.tscn`; model: `Company/Extinguisher` in `main.tscn`; held item: `game-core/PlayerProfile.cs` |
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
