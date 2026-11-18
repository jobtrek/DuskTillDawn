# Game core (`game-core/`, `game-core.tests/`)

Plain C# class library, namespace `DuskTillDawn.Core`. No Godot types. Holds the
game state and rules so they run and test without the engine.

| File | Role |
|---|---|
| `GameState.cs` | The single game state: clock, SLA, Heat, Traffic, Disk, Power, Points, `NightStatus`. `Tick(realSeconds)` advances time |
| `Challenge.cs` | Abstract Incident (legacy name; the domain term is Incident): alert, `Room`, points, duration, Pending → Running → Succeeded/Failed |
| `PlayerProfile.cs` | Name, points, room |
| `SunPosition.cs`, `MoonPosition.cs` | Sky positions from the hour of day |

## Rules for changes

- New game state or rules go here, never in a Godot node.
- State is changed through methods (`DrainSla`, `LosePoints`, `Tick`), not public setters.
- Godot reads it through `Main.State`.

## Tests

- `game-core.tests/` (xUnit), run by CI with `dotnet test`.
- Agents do not add tests unless asked ([rules/code.md](../rules/code.md)).
  Existing tests must keep passing.
