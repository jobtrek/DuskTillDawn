# Game core (`game-core/`, `game-core.tests/`)

Plain C# class library, no Godot types. Holds the game state and rules so they run and test
without the engine. One folder per feature; namespace follows the folder (`DuskTillDawn.Core.Incidents`).

| Folder | Files and role |
|---|---|
| `Night/` | `GameState` (the root: clock, SLA, Heat, Traffic, Disk, Power, Points, `NightStatus`; `Tick(realSeconds)`; `DrainSlaOverTime`; night bonus paid once at 08:00), `NightPlan` (per night 1 to 5: Incident kinds, spawn interval 180 s → 75 s, Fake e-mails; table in `docs/player-flow/challenges.md` §4), `NightReport`, `NightLog` (CSV sample every 30 in-game min), `PlayerProfile` (name, points never below 0), `HeldItem` (one item at a time) |
| `Incidents/` | `Incident` (abstract: alert, `Room`, points, duration, `IncidentState` Pending → Running → Succeeded/Failed; `SlaDrainPerMinute` 0.2 by default when it hits no Server; `Hit(servers)` sets `Targets`; `Warning` text), `PlayerAction`, the seven kinds (`SecurityAttack`, `BrokenDisk`, `OverheatingServer`, `Fire`, `UnpluggedCable`, `PhishingEmail`, `ElectricalFault`), `PointsRule` (+50 % fast repair, −50 % on a failure, `NightBonus` +200 plus 1 per SLA %) |
| `Incidents/` (engine) | `Game` (abstract Incident engine: players, Incidents, failures, solved; `SlaDrainPerMinute` = distinct Clients on broken Servers × `SlaDrainPerClient` plus flat drain of untargeted ones; sets `Server.Status` Down/Ok; raises `IncidentLaunched` → `GameState.ReceiveWarning`), `SoloGame` (lost after 3 failures), `IncidentSpawner` (one Incident per interval, skips a kind still running or ended less than one interval ago), `IncidentPool` (`PickRandom`, optional `Random`) |
| `Power/` | `ServerRoom` (Heat 18–55 °C, `HeatZone`, battery, Traffic, UPS, Auto-shutdown, Eco Mode, `RoomPowerState`; ticked by `Main`), `ElectricalFaultRisk` (faults likelier under high power draw; a failed fault launches a Fire) |
| `Mails/` | `Mail`, `MailKind`, `WarningText`, `Inbox` (`emails.json` schedule, arrival, read and report state; Warning mail linked by `Mail.IncidentId`), `PhishingIncidents` (a Phishing mail launches a `PhishingEmail`, `Report(mailId)` repairs it). Plural so the namespace does not clash with the `Mail` class |
| `Weather/` | `Weather` (kinds and rain), `WeatherSystem` (changes every 120 s, `SkipToNext` = F4; parked to M2), `LightningDamage` (strike → `DamagedSolarPanel`), `SunPosition`, `MoonPosition` |
| `Servers/` | `Server` (name, Clients, load, status), `ServerHealth` (Ok / Warn / Down), `ServerRack` (8 fixed Servers, `PickRandom`, seeded by `GameState(seed)`) |
| `Building/` | `Room` (Roof, Control, Servers) |

Built so far vs v1 target ([game/rules-v1.md](../game/rules-v1.md)): 7 Incidents; `Main` spawns
the night 1 plan (`NightPlan`) and lets `ElectricalFaultRisk` spawn faults.

## Rules for changes

- New game state or rules go here, in the folder of their feature, never in a Godot node.
- State is changed through methods (`DrainSla`, `LosePoints`, `Tick`), not public setters.
- Godot reads it through `Main.State`.

## Tests

- `game-core.tests/` (xUnit), run by CI with `dotnet test`.
- Agents do not add tests unless asked ([rules/code.md](../rules/code.md)).
  Existing tests must keep passing.
