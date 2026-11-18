# Monitoring app (computer)

Opens from the `MonitoringPressed` signal on the desk computer. Called **Dashboard** in [game-v1.md](../game-v1.md) §10. Values and rules: §5 and §7. Look and feel: [agents_gui_guide.md](../agents_gui_guide.md).

## Purpose

- Show the 5 simulation values so the player sees trouble coming.
- Show active incidents and **which floor** to go to (the player diagnoses here, fixes in the room).

Must be readable in under 2 seconds.

## Layout

A window inside `Map/Screen`, above `Icons`, below `BlockReveal`.

```text
┌ Monitoring ─────────────────────────────── 02:40 ── [X] ┐
│ SLA      ██████████████████░░  91 %                     │
│ Power    ████████████░░░░░░░░  60 %   ~~~~~\___         │
│ Heat     ██████████████████░░  84 °C  ___/~~~~   ⚠      │
│ Traffic  ████████░░░░░░░░░░░░  40 %   ~~~~~~~~          │
│ Disk     ████░░░░░░░░░░░░░░░░  22 %   ________          │
├─────────────────────────────────────────────────────────┤
│ ALERTS                                                  │
│ 🔴 Cooling failure        Basement       Active         │
│ 🟠 Dirty solar panels     Roof           Warning        │
└─────────────────────────────────────────────────────────┘
```

| Zone | Content |
|---|---|
| Top | In-game time. |
| Values | One row per value: name, bar, number, small line graph of recent history. |
| Alerts | One row per incident not Resolved: name, floor, state. Worst state first. Exception: Phishing only appears from Active, as "Account compromised" (its Warning only lives in the Mail app). |

## Colours

| Colour | Value rows | Alert rows |
|---|---|---|
| Green | Normal | – |
| Orange | Close to a limit | Warning |
| Red | Past a limit | Active |
| Dark red, blinking | – | Escalated |

Limits from §5: Power 0 % = servers offline, Heat 80 °C = fire risk / 95 °C = fire, Traffic 100 % = requests dropped, Disk 100 % = crash, SLA 0 % = game lost.

## Data (read from `GameState`)

- `Power`, `Heat`, `Traffic`, `Disk`, `Sla`, in-game time.
- Incidents: name, floor, state.
- Listens to `ValuesChanged` and `IncidentChanged` to redraw.

## Signals

| Signal | When |
|---|---|
| `CloseRequested()` | [X] or Escape. |

The app has no actions in v1. It only shows state.

## Open questions

1. Orange thresholds for Power, Traffic, Disk, SLA are not in game-v1. Proposal: put them in a data file and tune in playtests.
2. How much history does the line graph show? Proposal: last 2 in-game hours.
3. The HUD (`main/hud_ui/`) already shows the clock, SLA and points. Should it also show Power, Heat, Traffic and Disk, or stay small?
