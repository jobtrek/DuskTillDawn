# Game core (`game-core/`, `game-core.tests/`)

Plain C# class library, namespace `DuskTillDawn.Core`. No Godot types. Holds the
game state and rules so they run and test without the engine.

| File | Role |
|---|---|
| `GameState.cs` | The single game state: clock, SLA, Heat, Traffic, Disk, Power, Points, `NightStatus`. `Tick(realSeconds)` advances time; `DrainSlaOverTime` turns a per-minute SLA loss into real seconds; the night bonus is paid once at 08:00 |
| `Challenge.cs` | Abstract Incident (legacy name; the domain term is Incident): alert, `Room`, points, duration, Pending → Running → Succeeded/Failed; `SlaDrainPerMinute` (0.2 by default, virtual) is the SLA lost while it runs when it hits no Server; `Hit(servers)` sets its `Targets` |
| `SecurityAttack.cs`, `BrokenDisk.cs`, `OverheatingServer.cs`, `Fire.cs`, `UnpluggedCable.cs`, `PhishingEmail.cs`, `ElectricalFault.cs` | The seven challenges: each one sets its alert, room, points and the `PlayerAction` that fixes it; `BrokenDisk(rack)` hits one random Server of the `ServerRack` |
| `ServerRoom.cs` | Heat (18–55 °C, zones), battery, Traffic, UPS, Auto-shutdown, Eco Mode; ticked by `Main` during the night |
| `ElectricalFaultRisk.cs` | Random Electrical faults, likelier under high power draw; a failed fault launches a Fire |
| `NightLog.cs` | Samples every 30 in-game min and the night result, as CSV |
| `ChallengeSpawner.cs` | Launches a challenge every interval (default 20 s), kinds in turn, driven by `Tick`; skips a kind still running or that ended less than one interval ago; stops when the game is finished. `Main` passes one random-pick kind, so one incident runs at a time |
| `NightPlan.cs` | Per night (1 to 5): which challenges can spawn and the spawn interval (180 s down to 75 s); table in `docs/player-flow/challenges.md` §4 |
| `StoreInstancesOfIncidents.cs` | Holds the incident factories and picks one at random (`PickRandom`); optional `Random` for a fixed seed |
| `Servers/` | Servers and their Clients: `Server` (name, Client names, load, status), `ServerHealth` (Ok / Warn / Down), `ServerRack` (8 fixed Servers set in code, `PickRandom`; optional `Random`, seeded by `GameState(seed)`) |
| `PlayerProfile.cs` | Name, points (never below 0), room |
| `PointsRule.cs` | Points for a challenge: +50 % for a fast repair, −50 % for every player on a failure; `NightBonus` gives +200 plus 1 per SLA % for a won night |
| `Game.cs` | Abstract game: players, challenges, failures, solved. `Tick` moves the challenges; `SlaDrainPerMinute` = distinct Clients on broken Servers × `SlaDrainPerClient` (each Client once), plus the flat drain of running ones with no Targets; sets each target's `Server.Status` to Down while it runs, Ok once it is fixed or failed; raises `ChallengeLaunched` (Main sends it to `GameState.ReceiveWarning`); each mode says when it is lost or finished |
| `SoloGame.cs` | One player; lost after 3 failed challenges |
| `DuoGame.cs` | Two-player race: ends after `ChallengeCount` challenges, `Winner` has the most points (null on a tie). Parked (contract: 1 player), not wired in Godot |
| `Inbox.cs`, `Mail.cs` | News and Phishing schedule (`emails.json`), arrival, read and phishing-report state; `GameState.ReceiveWarning` adds an Incident's Warning mail (`Challenge.Warning`, linked by `Mail.ChallengeId`) |
| `HeldItem.cs` | What the player carries (one item at a time) |
| `SunPosition.cs`, `MoonPosition.cs` | Sky positions from the hour of day |
| `Weather.cs`, `WeatherSystem.cs` | Weather kinds (Clear, Cloudy, Rainy, Storm) and their rain; `GameState.Weather` changes every 120 s by weighted chance, rain blends in slowly; `SkipToNext` is the debug skip (F4). Built early: parked to M2 (`docs/game-v1.md`) |

Built so far vs v1 target ([game/rules-v1.md](../game/rules-v1.md)): 7 challenges, states
Pending → Running → Succeeded/Failed; `Main` spawns the night 1 plan (`NightPlan`): `BrokenDisk` (hits one random Server),
`SecurityAttack`, `OverheatingServer`, and lets `ElectricalFaultRisk` spawn faults.

## Rules for changes

- New game state or rules go here, never in a Godot node.
- State is changed through methods (`DrainSla`, `LosePoints`, `Tick`), not public setters.
- Godot reads it through `Main.State`.

## Tests

- `game-core.tests/` (xUnit), run by CI with `dotnet test`.
- Agents do not add tests unless asked ([rules/code.md](../rules/code.md)).
  Existing tests must keep passing.
