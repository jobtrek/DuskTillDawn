# Server Progression — Mini Spec

Players grow their setup from one laptop to a multi-rack server room. Each stage is unlocked by a need, not just money.

## Stages

| # | Setup | Capacity | New mechanic |
|---|-------|----------|--------------|
| 1 | Single laptop | 1 service | Basic uptime |
| 2 | 3-5 laptops on a shelf | ~5 services | Cabling and heat |
| 3 | Tower/mini servers + switch | ~15 services | Networking |
| 4 | Single server rack | ~50 services | Power and cooling |
| 5 | Multiple racks | 200+ services | Redundancy and automation |

## Stage 1: Single Laptop
One laptop runs one service. The player fixes everything by hand, which teaches the core loop.

## Stage 2: Multiple Laptops
Several laptops sit on a shelf, each lying flat with the screen facing down toward the floor, and run more services. Cable mess and heat appear as the first real problems (face-down laptops trap heat). Each laptop has a status LED: green = healthy, orange = hot or overloaded, red = failed, off = no power.

## Stage 3: Tower Servers
Small servers connect through a switch. Network faults and the first real outages appear here. Each server and the switch have a status LED (same colors as stage 2); switch port LEDs blink on traffic.

## Stage 4: Server Rack
One rack holds rack-mounted servers. Power (UPS, PDU) and cooling become things the player must manage. Each server unit has a status LED, plus one rack-level LED for power and cooling. Power chain: wall → UPS (battery backup, survives a power cut for a few minutes) → PDU (splits power to servers, trips if overloaded) → servers.

## Stage 5: Multiple Racks
Several racks fill a room. Redundancy and automation matter, since one person can't hand-fix everything. Each unit keeps its LED, and each rack gets a top-of-rack LED showing its worst unit, so problems are visible from across the room.

## Unlock Rules
- **Need-based:** a stage unlocks when the player hits a wall (overheating, rejected contracts, hardware failures).
- **Money-based:** the player must also afford the upgrade cost.
- **Both required:** this keeps upgrades feeling earned instead of a checkbox.

## Mini-Upgrades (inside a stage)
Small purchases like RAM, a better switch, a UPS or an extra fan give frequent wins between big jumps.

## Old Hardware
Retired machines stay useful as test boxes or backup nodes, so growth never feels like pure replacement.

## Failure Stakes
Failure cost scales with the stage. A dead laptop is minor, a dead rack PSU is a crisis.

## Pacing
- Contracts set the tempo: their size and frequency push the next upgrade.
- **Full run target: 4-5 hours.**
- Time spent per stage: S1 ~15 min, S2 ~35 min, S3 ~60 min, S4 ~80 min, S5 ~80 min (about 4.5 h).
- Each stage is longer than the last because it adds a new mechanic to learn.
- **Too fast:** upgrade cost is under 2 contracts of income.
- **Too slow:** the player saves over 3 sessions with no new event.
- Random events (heatwave, client surge) break up long saving stretches.

## Economy (starting values, tune in playtests)
Coins arrive continuously while services run. Income per minute rises with each stage, but upgrade prices rise faster, so each stage feels like saving up.

| Stage | Income/min | Time in stage | Earned | Upgrade price to next stage | Unlock wall (the need) |
|---|---|---|---|---|---|
| 1 | 10 | ~15 min | ~150 | 100 | the single service is full, contracts rejected |
| 2 | 25 | ~35 min | ~875 | 600 | laptops overheat or fail, 5 services full |
| 3 | 60 | ~60 min | ~3,600 | 2,500 | network outages, switch maxed, 15 services full |
| 4 | 130 | ~80 min | ~10,400 | 7,000 | rack full (50 services), power or cooling limit |
| 5 | 250 | ~80 min | ~20,000 | end goal: fill the room | none, final stage |

- About 70% of earnings go to the next stage. The rest goes to mini-upgrades (RAM, switch, UPS, fan).
- Income is about 2.5x per stage. Price is about 5x per stage.
- Price rule: upgrade price = income/min x minutes in stage x 0.7.
- Failures cost coins too (repairs, lost contracts), so a bad stretch delays the next stage.
