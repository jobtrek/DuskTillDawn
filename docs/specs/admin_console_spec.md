# Admin console app (computer)

A 4th app on the desk computer, next to Mail, Monitoring and the AI helper. It holds the **fix actions** for the computer Incidents. Fix steps come from [game-v1.md](../game-v1.md) §8. Look and feel: [agents_gui_guide.md](../agents_gui_guide.md).

Needs a 4th desktop icon (16×16 pixel art) and a new `AdminPressed` signal on `ComputerMenu`.

## Purpose

Mail and Monitoring only show things. The Admin console is where the player **does** the fix once they know the cause.

| Incident | Fix steps in this app |
|---|---|
| #1 Expired TLS certificate | Certificates → Renew → Reload web server |
| #2 DDoS | Firewall → Enable traffic filtering |
| #3 Vulnerable dependency | Packages → Update package → Restart app |
| #4 Bad update | Updates → Roll back → Reboot servers |

#8 Phishing is fixed in the Mail app (Report), not here.

## Layout

A window inside `Map/Screen`, above `Icons`, below `BlockReveal`.

```text
┌ Admin ────────────────────────────────────────── [X] ┐
│ Certificates │  shop.example   expires in 0 d  [Renew] │
│ Firewall     │                                [Reload] │
│ Packages     │                                         │
│ Updates      │                                         │
└──────────────┴─────────────────────────────────────────┘
```

| Zone | Width | Content |
|---|---|---|
| Tabs | 25 % | One tab per system. A red dot on a tab whose system has an Active Incident. |
| Panel | 75 % | The state of that system and its action buttons. |

- Buttons always work, even with no Incident (e.g. restarting the app when nothing is wrong).
- Each step must be done in order. A later step is greyed until the earlier one is done.

## Signals (UI emits, the game decides)

| Signal | When |
|---|---|
| `ActionPressed(string system, string action)` | Any action button. The game advances or resolves the Incident. |
| `CloseRequested()` | [X] or Escape. |

## Open questions

1. Does doing an action when nothing is wrong cost anything? (e.g. rebooting servers costs SLA while they are down.)
2. Should an action take time (a progress bar), so the player feels the pressure?
3. Does the red dot on a tab give away too much? It skips the "diagnose" step.
