# Game rules (v1)

Numbers are starting values; the full tables are in `docs/game-v1.md` §5–§9.

## Time

- Night 20:00 → 08:00. 1 in-game hour = 50 real s (night = 10 min).
- Day 08:00 → 20:00, SLA frozen. Implemented in `game-core/GameState.cs`.

## Simulation: 5 values

| Value | Range | Key rule |
|---|---|---|
| Power | 0–100 % | Panels charge, servers drain. 0 % = servers offline |
| Heat | 20–100 °C | > 80 fire risk, 95 fire starts |
| Traffic | 0–100 % | > 100 % requests dropped |
| Disk | 0–100 % | 100 % = app crash |
| SLA | 0–100 % | The score. Starts at 100 each night |

Three cascades only: dirty panels → less Power; cooling fails → Heat → fire;
DDoS → load → Heat and Power draw.

## Win and lose

- Lose: SLA hits 0 % (bankruptcy). Win a night: reach 08:00 with SLA > 0.
- SLA drain/min: Clients on broken Servers × 0.05 % (each Server once) + flat rate of Incidents with no Server.

## Incident life cycle

`Dormant → Warning → Active → Escalated → Resolved`

- Warning → Active after 60 s; Active → Escalated after 3 min (drain doubles).
- Fixing at Warning gives +50 % points.
- A fix only happens with the player standing in the Incident's room.

## The 8 Incidents

TLS cert, DDoS, vulnerable dependency, bad update, phishing mail (Control room);
dirty solar panels (Roof); cooling failure → fire, failing drive (Basement).
Fix steps, upgrades, night table: `docs/game-v1.md` §8–§9.
