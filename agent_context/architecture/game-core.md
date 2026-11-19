# Game core (`game-core/`, `game-core.tests/`)

Plain C# class library, namespace `DuskTillDawn.Core`. No Godot types. Holds the
game state and rules so they run and test without the engine.

| File | Role |
|---|---|
| `GameState.cs` | The single game state: clock, SLA, Heat, Traffic, Disk, Power, Points, `NightStatus`. `Tick(realSeconds)` advances time |
| `Challenge.cs` | Abstract Incident (legacy name; the domain term is Incident): alert, `Room`, points, duration, Pending → Running → Succeeded/Failed |
| `SecurityAttack.cs`, `BrokenDisk.cs`, `OverheatingServer.cs`, `Fire.cs`, `UnpluggedCable.cs`, `PhishingEmail.cs` | The six challenges: each one sets its alert, room, points and the `PlayerAction` that fixes it |
| `ChallengeSpawner.cs` | Launches a challenge every interval (default 20 s), kinds in turn, driven by `Tick`; stops when the game is finished |
| `PlayerProfile.cs` | Name, points (never below 0), room |
| `PointsRule.cs` | Points for a challenge: +50 % for a fast repair, −50 % for every player on a failure |
| `Game.cs` | Abstract game: players, challenges, failures. `Tick` moves the challenges; each mode says when it is lost or finished |
| `SoloGame.cs` | One player; lost after 3 failed challenges |
| `DuoGame.cs` | Two-player race: ends after `ChallengeCount` challenges, `Winner` has the most points (null on a tie) |
| `SunPosition.cs`, `MoonPosition.cs` | Sky positions from the hour of day |

## Rules for changes

- New game state or rules go here, never in a Godot node.
- State is changed through methods (`DrainSla`, `LosePoints`, `Tick`), not public setters.
- Godot reads it through `Main.State`.

## Tests

- `game-core.tests/` (xUnit), run by CI with `dotnet test`.
- Agents do not add tests unless asked ([rules/code.md](../rules/code.md)).
  Existing tests must keep passing.
