# New mail toast

How a new Mail reaches the player wherever they are. Part of issue #46. Mail rules: [mail_app_spec.md](mail_app_spec.md). Look and motion: [agents_gui_guide.md](../agents_gui_guide.md).

## Flow

```text
Main._Process(delta)
   │
   ▼
GameState.Tick(seconds) ── clock passes a scheduled time
   │
   ▼
Inbox.Deliver(mail) ── adds mail, IsRead = false
   │
   ▼
GameState.MailReceived?.Invoke(mail)        (C# event, game-core)
   │
   ├──► Hud.OnMailReceived ──► ShowToast()   (anywhere in the building)
   ├──► ComputerMenu ────────► unread badge +1
   └──► MailApp (if open) ───► new row on top
```

## Who owns what

| Piece | Lives in | Job | Never does |
|---|---|---|---|
| `MailSchedule` | data file | Which Mail arrives at which in-game time | — |
| `Inbox` | `game-core` | Holds the Mails and the unread count | Know about Godot |
| `GameState.Tick` | `game-core` | Delivers due Mails, raises `MailReceived` | Show anything |
| `Hud` | `main/hud_ui` | Shows the toast | Read Mail content |

## Event contract

```csharp
public event Action<Mail>? MailReceived;
```

- Passes the whole `Mail`: `MailApp` needs it to add a row. The HUD ignores the content.
- Raised once per Mail, in arrival order.
- Listeners subscribe in `_Ready` and unsubscribe in `_ExitTree`.

## Toast rules

| Rule | Value |
|---|---|
| Text | "New mail". No sender, no subject ([mail_app_spec.md](mail_app_spec.md) "New Mail" rule). |
| Show | Pop with a quick scale, no backdrop ([motion.md](../gui/motion.md)). |
| Stay | 3 s (starting value, tune in playtests). |
| Second Mail while showing | Restart the 3 s timer, text becomes "2 new mails". |
| Hide | Fade out. |
| Position | Anchored bottom-right. Never over the status bar or the end-of-night banner. |
| Input | `MouseFilter = Ignore`, never steals clicks. |

## Tests (`game-core.tests`)

1. Tick past a Mail's time: `MailReceived` fires once, unread count +1.
2. Tick past two Mail times in one tick: the event fires twice, in order.
3. Tick again: no duplicate event.

The toast itself has no unit test. Check it in the game.

## Open questions

1. Toast duration of 3 s and the "2 new mails" stacking are proposals. Confirm with the team.
