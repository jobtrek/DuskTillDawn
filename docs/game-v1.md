# Dawn to Dusk: game v1

The first playable version. It takes the ideas from [ideas/](ideas/) and cuts them down to what we can build and playtest first. Everything not listed here is in [section 11: parked](#11-parked-ideas-not-in-v1).

All numbers are **starting values**. Put them in data files and tune them in Friday playtests.

---

## 1. Pitch

You are the only sysadmin on the night shift. Servers run on solar power. Things break, some by accident, some because someone is attacking you. Keep the company alive until 06:00.

**v1 = single player, 3 floors, 5 nights, 8 incidents, one upgrade shop.**

## 2. Pillars

Every v1 feature must support at least one. If it doesn't, it waits.

1. **Pressure, not panic.** Always slightly too much to handle, never unfair.
2. **Real incidents, simplified.** Each incident maps to something that really happened.
3. **Everything is connected.** Fixing the root cause beats fixing symptoms.

## 3. The three loops

| Loop | Scale | What happens |
|---|---|---|
| **Core** | ~1 minute | Alert → diagnose on the computer → walk to the right floor → fix |
| **Night** | ~10 min | Survive from 22:00 to 06:00 (in-game) with the SLA above zero |
| **Meta** | across nights | Points → shop upgrades → harder next night |

## 4. The building

| Floor | What is there | What the player does here |
|---|---|---|
| **Roof** | Solar panels, radio antenna | Clean panels, realign antenna |
| **Control room** | Main computer (dashboard, mail, AI helper) | Diagnose, change settings, read warnings |
| **Basement** | Servers, cooling, cables | Fight fires, swap drives, replug cables |

- The player is one character walking in isometric voxel view.
- Moving between floors takes **8 seconds** (stairs). While you are away, the dashboard keeps running and incidents keep coming. This is the main pressure.
- The player can only **fix** an incident when standing in its room. They can **see** everything from the computer.

## 5. The simulation (5 values)

Everything the player sees on the dashboard comes from these.

| Value | Range | Rule |
|---|---|---|
| **Power** | 0–100 % | Panels charge the battery. Servers drain it. At 0 %, servers go offline. |
| **Heat** | 20–100 °C | Rises with server load, falls with cooling. Above 80 °C: fire risk. At 95 °C: fire starts. |
| **Traffic** | 0–100 % of capacity | Normal ~40 %. Above 100 %, requests are dropped. |
| **Disk** | 0–100 % | Fills slowly from logs. At 100 %, the app crashes. |
| **SLA** | 0–100 % | The score. See section 6. |

### Cascade rules (v1 has exactly three)

| If... | Then... |
|---|---|
| Panels are dirty or damaged | Battery charges slower, so Power drops, so servers slow down |
| Cooling fails | Heat rises, so Fire |
| Traffic spike (DDoS) | Load rises, so Heat and Power draw both rise |

These three are enough to teach "everything is connected". More cascades come later.

## 6. Win and lose

- **SLA** starts at 100 % each night.
- Servers up and healthy: SLA stays flat.
- Each active incident drains SLA every in-game minute (rate is in the incident's data file, see section 8).
- Servers fully down: **extra −1 % per in-game minute**.
- **Lose the game:** SLA reaches 0 %. The company goes bankrupt.
- **Win a night:** reach 06:00 with SLA above 0 %.

### Time

- A night runs 22:00 → 06:00. One in-game hour = **75 real seconds** (night = 10 min).
- The player can speed up ×2 while nothing is wrong.
- Night report shows: SLA left, incidents solved, incidents failed, points earned.

## 7. Incident life cycle

Every incident follows the same states:

```
Dormant → Warning → Active → Escalated → Resolved
```

| State | Meaning | Player sees |
|---|---|---|
| **Warning** | Early sign, no damage yet. Solving now costs the least. | Email or dashboard hint |
| **Active** | Damage is happening, SLA drains. | Alarm, red on dashboard |
| **Escalated** | Not fixed in time. Drain doubles and it may spawn a cascade. | Louder alarm, extra effects |
| **Resolved** | Fixed. Stops draining. | Green, points |

- Warning → Active after **60 s** if ignored.
- Active → Escalated after **3 min** if ignored.
- Fixing at Warning gives **+50 % points**. This rewards reading the dashboard.

## 8. The 8 incidents for v1

One per row. Each gets a spec file in `docs/design/incidents/` and a data file in `data/incidents/`.

| # | Incident | Floor | Real event it teaches | Fix (player actions) | Counter-upgrade |
|---|---|---|---|---|---|
| 1 | **Expired TLS certificate** | Control room | Forgotten renewals | Open cert manager → renew → reload web server | Auto-renewal |
| 2 | **DDoS** | Control room | Cloudflare attacks | Enable traffic filtering | CDN shield |
| 3 | **Vulnerable dependency** | Control room | Log4Shell | Read mail → update package → restart app | Vulnerability scanner |
| 4 | **Bad update** | Control room | CrowdStrike | Roll back the update → reboot servers | Staged rollouts |
| 5 | **Dirty solar panels** | Roof | Snow / dust | Walk up → sweep panels (hold button) | Self-cleaning panels |
| 6 | **Cooling failure → fire** | Basement | OVHcloud fire | Restart AC. If fire started: use extinguisher | Second AC / sprinklers |
| 7 | **Failing hard drive** | Basement | Drive failures | Find red drive → swap with spare | RAID |
| 8 | **Phishing email** | Control room | Social engineering | Read mail → spot the fake → report it | 2FA |

Notes:
- 5 of 8 are on the computer, so the core loop works even before the voxel world is ready.
- #6 is the showcase: it is a cascade (cooling → heat → fire) that needs two floors.
- The supply-chain backdoor (xz-utils) is parked, because it needs the delivery dock.

### Incident spec (what each data file holds)

| Field | Example (TLS cert) |
|---|---|
| Floor | Control room |
| Difficulty | 1 / 3 |
| Warning text (mail) | "Customers report a security warning" |
| Dashboard effect | HTTPS errors spike, traffic drops |
| SLA drain per minute | Active 0.4 %, Escalated 0.8 % |
| Fix steps | 3 |
| Points | 100 |
| What the player learns | "Certificates expire. Automate renewal." |

## 9. Nights, difficulty and the shop

### Night progression (simple mode)

| Night | Incidents available | Spawn rate | Note |
|---|---|---|---|
| 1 | 1, 5 | 1 every ~3 min | Tutorial. AI helper explains each. |
| 2 | 1, 2, 5, 7 | 1 every ~2.5 min | First DDoS |
| 3 | 1–5, 7 | 1 every ~2 min | First bad update |
| 4 | 1–8 | 1 every ~90 s | Cascades unlocked |
| 5 | 1–8 | 1 every ~75 s, two can overlap | Final night |

Survive night 5 and you win the run.

### Points and shop

- Points per solved incident (see data file) plus **+200** for surviving the night plus **+1 per SLA %** left.
- Shop opens between nights. Items are the counter-upgrades from the table in section 8.
- Rule: a counter-upgrade removes or shortens its incident, but **never removes the lesson**. Example: after buying auto-renewal, the TLS incident can no longer happen, and the mail explains why.
- Price range: 300 to 1500 points. Expect about 700 points per night, so the player can afford 1–2 upgrades per night.
- Upgrades persist for the rest of the run. Losing resets everything.

### Infinite mode (after simple mode works)

Same rules, but the spawn rate keeps rising every hour of play and nights never end. The score is "how many in-game hours survived". Not in the first playable.

## 10. The computer

Three apps. All must be readable in under 2 seconds.

| App | v1 content |
|---|---|
| **Dashboard** | 5 graphs (Power, Heat, Traffic, Disk, SLA) with warning colors |
| **Mail** | Warnings for each incident, tech news (flavor + hints), phishing mails |
| **AI helper** | Our own character (not a real brand). Gives a hint when the player has been stuck for 60 s. Never solves it for them. |

## 11. Parked ideas (not in v1)

Kept in [ideas/](ideas/), in rough order of when to bring them back.

| Idea | Why parked | Comes back in |
|---|---|---|
| **Infinite mode** | Easy once nights work, not needed to test fun | M2 |
| **Battery shed + generator yard** | Great depth, but needs outdoor area, new mini-games and new art | M2 |
| **Outdoor areas** (fiber trench, substation, gate) | Same as above | M2 |
| **Weather** (snow, storms, floods, lightning, animals) | Needs outdoor world | M2 |
| **Delivery dock + supply-chain attacks** | Very good idea, but a full area plus mini-game | M2/M3 |
| **Daytime maintenance rounds** | Doubles the content. v1 uses only the shop between nights. | M3 |
| **Two-player co-op** | Needs network sync. Design incidents as 1-player first. | M3 |
| **Character customization** | Cosmetic, no effect on testing the core loop | M3 |
| **Bigger companies / more levels** | Needs the base game balanced first | M4 |
| **Tablet, drone, golf cart, flashlight** | Only matter once outdoors exists | with outdoors |
| **Sleep / day cycle with night interrupts** | Replaced by the shop screen | M3 |

**Open question from the original idea notes:** the last line, "2 player mechanic, one attacks and one attacks", looks like a typo. If one player is meant to *play the attacker* (a versus mode), that is a totally different game and needs its own design. Not in v1.

## 12. Definition of done for v1

Someone who has never seen the game can:

1. Play night 1 without help and understand what to do.
2. Lose by bankruptcy and win by surviving night 5.
3. Name at least 2 real-world incidents they learned about.
4. Say which upgrade they want to buy next.

## 13. Cut order if we run out of time

Cut from the bottom first:

1. Nights 4–5 cascade overlap
2. AI helper (keep mail hints)
3. Incidents #4 and #8
4. Voxel art (use grey-box)

Never cut: SLA, the three-floor walk, the dashboard, and the shop.
