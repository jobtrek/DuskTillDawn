# AI helper app (computer)

Opens from the `HelpPressed` signal on the desk computer. Rules from [game-v1.md](../game-v1.md) §9, §10 and §13. Look and feel: [agents_gui_guide.md](../agents_gui_guide.md).

## Purpose

- A hint character, **our own, not a real brand**. The desktop icon is labelled "ChatGPT" today and must be renamed.
- Gives a hint when the player has been stuck on an incident for **60 s**.
- **Never solves the incident** for the player.
- Night 1 (tutorial): explains each incident when it appears.
- Second thing to cut if we run out of time (§13). Build after Mail and Monitoring.

No real AI or network calls. All text comes from the incident data files.

## Layout

A window inside `Map/Screen`, above `Icons`, below `BlockReveal`.

```text
┌ <Helper name> ───────────────────────────── [X] ┐
│  ┌──────┐   ┌────────────────────────────────┐  │
│  │ pixel│   │ The basement is getting hot.   │  │
│  │ face │ ◄ │ Have you checked the AC unit?  │  │
│  └──────┘   └────────────────────────────────┘  │
│                                                 │
│  Cooling failure · Basement        [ Got it ]   │
└─────────────────────────────────────────────────┘
```

| Zone | Content |
|---|---|
| Portrait | 32×32 pixel-art face, small idle bounce. |
| Speech bubble | One hint, max ~2 short lines. |
| Footer | Which incident the hint is about, "Got it" button. |

## What it says

| Situation | Text |
|---|---|
| No incident, or nobody stuck yet | A short idle line (flavour). |
| An incident has been unsolved for 60 s or more | That incident's hint. Oldest stuck incident first. |
| Night 1, new incident | The tutorial explanation for that incident. |

## Data (read from `GameState`)

- Incidents: name, floor, state, time since it started.
- Per incident in its data file: `Hint` and `TutorialText`. **These 2 fields are new**; the incident spec table in game-v1 §8 needs them added.

## Signals

| Signal | When |
|---|---|
| `HintAcknowledged(int incidentId)` | "Got it". Game can stop showing that hint. |
| `CloseRequested()` | [X] or Escape. |

## Open questions

1. Name and look of the character.
2. Does the hint only appear when the player opens the app, or does a small popup push it? Proposal: a dot on the desktop icon, text only in the app.
3. Does "stuck for 60 s" count from Warning or from Active?
4. Does using a hint cost points?
