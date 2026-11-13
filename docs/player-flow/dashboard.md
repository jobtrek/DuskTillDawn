# Dashboard screen (mockup)

What the player sees on the computer in the control room. The alerts come from the [list of challenges](challenges.md).

## 1. One player

```text
┌──────────────────────────────────────────────────────────┐
│  DASHBOARD                          Points: 350          │
│                                     Failures: 1 / 3      │
├──────────────────────────────────────────────────────────┤
│  ALERTS                                                  │
│  🔴 A disk is not responding     Server room         0:45 │
│  🟠 Temperature too high         Server room         1:20 │
├──────────────────────────────────────────────────────────┤
│  SERVERS                                                 │
│  Server 1  🟢 OK                                         │
│  Server 2  🔴 Problem                                    │
│  Server 3  🟠 Hot                                        │
│  Server 4  ⚫ Down                                        │
└──────────────────────────────────────────────────────────┘
```

| Area | What it shows |
|---|---|
| Top | The player's points and the number of failures before losing |
| Alerts | The problem, the room to go to and the time left to repair |
| Servers | The state of each server, by color |

| Color | Meaning |
|---|---|
| 🟢 Green | Everything is fine |
| 🟠 Orange | Warning, it will break soon |
| 🔴 Red | Problem to repair now |
| ⚫ Black | Server is down |

## 2. Two players

Same screen, but the top shows both players' scores, and the alert says it is a race.

```text
┌──────────────────────────────────────────────────────────┐
│  DASHBOARD                Player 1: 400    Player 2: 250  │
├──────────────────────────────────────────────────────────┤
│  ⚔️  CHALLENGE RUNNING: Security attack             0:30 │
│      The first to fix it earns 150 points                │
└──────────────────────────────────────────────────────────┘
```

## To decide

- Is the dashboard only on the computer, or also always visible on screen (top right)?
