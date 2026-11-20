# Statistics

Numbers for balancing the shop. Source: [game-v1.md](game-v1.md). **TBD** = not decided yet, tune in playtests.

## Incidents

| # | Incident | Floor | Unlocks night | Points | Expected spawns per run |
|---|---|---|---|---|---|
| 1 | Expired TLS certificate | Control room | 1 | 100 | 5.4 |
| 5 | Dirty solar panels | Roof | 1 | TBD | 5.4 |
| 2 | DDoS | Control room | 2 | TBD | 3.7 |
| 7 | Failing hard drive | Basement | 2 | TBD | 3.7 |
| 3 | Vulnerable dependency | Control room | 3 | TBD | 2.7 |
| 4 | Bad update | Control room | 3 | TBD | 2.7 |
| 6 | Cooling failure → fire | Basement | 4 | TBD | 1.8 |
| 8 | Phishing email | Control room | 4 | TBD | 1.8 |

- Fixed at Warning: points × 1.5.
- SLA drain: only TLS is set (Active 0.4 %, Escalated 0.8 % per in-game minute). Others TBD.
- Expected spawns = sum over the 5 nights of (incidents that night ÷ incidents in the pool).

## Nights

| Night | Spawn every | Incidents | Pool size | Spawns per incident |
|---|---|---|---|---|
| 1 | 3 min | 3.3 | 2 | 1.7 |
| 2 | 2.5 min | 4 | 4 | 1 |
| 3 | 2 min | 5 | 6 | 0.83 |
| 4 | 90 s | 6.7 | 8 | 0.83 |
| 5 | 75 s | 8 | 8 | 1 |

## Income per night

`income = Σ incident points + 200 (survived) + SLA % left`

With 100 points per incident and 80 % SLA left: night 1 ≈ 610, night 2 ≈ 680, night 3 ≈ 780, night 4 ≈ 950.
The shop opens after nights 1–4, so a run earns **about 3,000 points**.

## Shop

- 8 counter-upgrades + 4 server stages. Prices TBD, range 300–1500.
- A counter-upgrade also removes that incident's future points.
