# Game core: world (`game-core/`)

The places and conditions incidents happen in: servers, roof panels, server room, weather, sky.
Same rules as [game-core.md](game-core.md): no Godot types, seeded by `GameState(seed)` where random.

| File | Role |
|---|---|
| `ServerRoom.cs` | Heat (18–55 °C, zones), battery, Traffic, UPS, Auto-shutdown, Eco Mode; ticked by `Main` during the night; it only reads the game, and `Main` calls `Game.FinishRecovered(room)` right after |
| `Servers/` | Servers and their Clients: `Server` (name, Client names, load, status), `ServerHealth` (Ok / Warn / Down), `ServerRack` (8 fixed Servers set in code, `PickRandom`; optional `Random`, seeded by `GameState(seed)`) |
| `SolarPanels/` | Roof solar panels: `SolarPanel` (name), `SolarArray` (9 panels PANEL-01..09, `PickRandom`, `PickRandomExcept(broken)`; seeded by `GameState(seed)`), owned by `GameState.SolarPanels` |
| `SunPosition.cs`, `MoonPosition.cs` | Sky positions from the hour of day |
| `Weather.cs`, `WeatherSystem.cs` | Weather kinds (Clear, Cloudy, Rainy, Storm) and their rain; `GameState.Weather` changes every 120 s by weighted chance, rain blends in slowly; `SkipToNext` is the debug skip (F4). In a Storm, `LightningStruck` fires about every 15 s; `ForceStrike` is the debug strike (F3). `LightningDamage.cs`: a strike has a 35 % chance to launch a `DamagedSolarPanel` on a random intact `SolarPanel` (its `Target`), even while other incidents run; `Game.RepairPanel` repairs one panel. Storms and lightning are v1; snow, floods and animals are parked to M2 (`docs/game-v1.md`) |
