# Mail app (computer)

Opens from the `MailPressed` signal on the desk computer. Content rules come from [game-v1.md](../game-v1.md) §7, §8 and §10. Look and feel: [agents_gui_guide.md](../agents_gui_guide.md). Terms (Incident, Mail): [CONTEXT.md](../../CONTEXT.md).

## Purpose

- Show the **Warning mail** for each Incident (early sign, cheapest moment to fix).
- Show **News mails** (tech news, flavour and hints).
- Let the player **report a Phishing mail** (Incident #8).

Must be readable in under 2 seconds. Fix actions for other Incidents live in the Admin console, not here.

## Layout

A window inside `Map/Screen`, above `Icons`, below `BlockReveal`.

```text
┌ Mail ─────────────────────────────────────── [X] ┐
│ ● IT Security   Customers report a warn… │ From: IT Security          │
│   Tech News     Log4Shell: patch now     │ Subject: Customers report… │
│ ● Admin Team    Reset your password      │                            │
│                                          │ Body text…                 │
│                                          │                            │
│                                          │        [ Report phishing ] │
└──────────────────────────────────────────┴────────────────────────────┘
```

| Zone | Width | Content |
|---|---|---|
| Inbox | 40 % | One row per Mail: unread dot, sender, subject (cut with `…`). Newest on top. |
| Reading pane | 60 % | Sender, subject, body. "Report phishing" button at the bottom. |

- A Phishing mail looks like a normal Mail in the list. The player spots it from the content (sender, wording).
- Selected row: lighter fill. Unread: white dot, bold subject. Mail of a Resolved Incident: greyed.

## Rules

| Rule | Value |
|---|---|
| "Report phishing" button | On every Mail. Disabled on a Mail once it has been reported. |
| Right report | Phishing Incident → Resolved. Shows "Reported ✓ +150" (Warning) or "+100" (Active/Escalated). Mail greys out. |
| Wrong report | Shows "Not phishing −25". Penalty once per Mail. Mail stays normal. |
| Ignored Phishing mail | 60 s → Active (a colleague clicked it, SLA drains). 3 min more → Escalated (drain doubles, no cascade). Reporting still resolves it. |
| New Mail | Unread count on the desktop Mail icon + a "New mail" HUD toast wherever the player is. No content in the toast. |
| Inbox life | Empties at the start of each night. Mails stay for the whole night. |

Points (100 base, −25 wrong report) live in the Incident data file, so they can be tuned in playtests.

## Data (read from `GameState`)

| Field | Type |
|---|---|
| `Id` | int |
| `Kind` | Warning, News or Phishing (never shown) |
| `From`, `Subject`, `Body` | string |
| `IncidentId` | int or null (News mails have none) |
| `IsRead`, `IsReported` | bool |

Listens to `MailReceived` to add new rows while open.

## Signals (UI emits, the game decides)

| Signal | When |
|---|---|
| `MailOpened(int id)` | Player selects a Mail. Game marks it read. |
| `ReportPressed(int id)` | Player presses "Report phishing". Game decides if it was right and tells the app the result. |
| `CloseRequested()` | [X] or Escape. |
