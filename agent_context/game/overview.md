# Game overview

Source of truth: `docs/contract.md`, `docs/game-v1.md`. This page is a map, not a spec.

## Pitch

You are the only sysadmin on a solar-powered night shift. Keep the company's
servers alive from 20:00 until 08:00. Things break, some by accident, some by attack.

## v1 scope

| Item | v1 |
|---|---|
| Players | 1 (single player only; co-op is parked to M3) |
| Floors | 3: Roof, Control room, Basement (cut-away, all on screen) |
| Nights | 5 (surviving night 5 wins the run) |
| Incidents | 8 |
| Computer apps | Monitoring, Mail, Admin console, AI helper |
| Shop | Between nights, counter-upgrades only |

## Pillars (every feature supports one, or it waits)

1. **Pressure, not panic.** Slightly too much to handle, never unfair.
2. **Real incidents, simplified.** Each Incident maps to a real event.
3. **Everything is connected.** Fixing the root cause beats fixing symptoms.

## Three loops

| Loop | Scale | What happens |
|---|---|---|
| Core | ~1 min | Alert → diagnose on the computer → walk to the floor → fix |
| Night | ~10 min | Survive 20:00 → 08:00 with SLA above 0 |
| Meta | across nights | Points → shop upgrades → harder next night |

## Look and feel

3D, fixed isometric camera, orthogonal projection, voxel art (`.glb` models).
Floor changes take 8 s by stairs: that walk is the main pressure.
Not in v1: everything in `docs/ideas/` (outdoors, weather, dock, co-op, infinite mode).
