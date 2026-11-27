# Challenges and player actions

Completes the [game diagram](game-states.md). The challenges come from [incidents.md](../ideas/incidents.md), simplified.

## 1. What each player can do (use cases)

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart LR
    J1["Player 1"] --> U1(["Read an alert on the dashboard"])
    J1 --> U2(["Move between rooms"])
    J1 --> U3(["Do a challenge to repair"])
    J1 --> U4(["See their points"])
    J2["Player 2 (two-player mode)"] --> U1
    J2 --> U2
    J2 --> U3
    J2 --> U4
```

## 2. List of challenges (proposal)

| #   | Problem            | Alert on the dashboard        | Where to repair              | What the player does                                       | Points |
| --- | ------------------ | ----------------------------- | ---------------------------- | ---------------------------------------------------------- | ------ |
| 1   | Security attack    | "Too many suspicious visits!" | Control room                 | Turn on the filter on the computer                         | 150    |
| 2   | Broken hard drive  | "A disk is not responding"    | Server room                  | Find the red disk and replace it                           | 100    |
| 3   | Overheating server | "Temperature too high"        | Server room                  | Restart the air conditioning                               | 100    |
| 4   | Fire               | "Fire alarm!"                 | Server room                  | Grab the extinguisher and put out the fire                 | 200    |
| 5   | Unplugged cable    | "A server is offline"         | Server room                  | Find the right cable and plug it back in                   | 100    |
| 6   | Fake e-mail        | "New urgent message"          | Control room                 | Spot that the e-mail is fake and report it                 | 100    |
| 7   | Electrical fault   | "Power fault in the basement" | Control room (Admin → Power) | Cut power to the faulty circuit. Ignored: it starts a Fire | 150    |

In two-player mode, both players get the same challenge at the same time. The first one to finish it gets the points.

### Carried items (proposal, to validate with the team)

- The player holds one item at a time, in the right hand of the first-person view. Only the extinguisher can be picked up for now.
- **E** next to the extinguisher picks it up. **E** near the fire with the extinguisher in hand puts it out.
- **Q** hangs the extinguisher back on its bracket.
- Code: `game-core/PlayerProfile.cs` (`Holding`), `dusk-till-dawn/main/fire/ExtinguisherSpot.cs`.

## 3. Points rule (proposal, to validate with the team)

| Case                                      | Points                                     | Example (challenge worth 100) |
| ----------------------------------------- | ------------------------------------------ | ----------------------------- |
| Repaired with at least half the time left | Base points + 50 %                         | +150                          |
| Repaired later                            | Base points                                | +100                          |
| Failed (time ran out)                     | Every player loses 50 % of the base points | −50                           |

- Points never go below 0.
- The +50 % bonus follows `docs/game-v1.md`: reading the dashboard early pays.
- Code: `game-core/PointsRule.cs`.

### Wrong action

- A repair action that fixes nothing while a challenge is running makes the player **busy for 5 s**: no repair works until it runs out, and the challenge timer keeps running. With no challenge running, an action costs nothing.
- Exception: reporting a mail in the Mail app is not blocked while busy. A wrong report already costs points, so it is not punished twice.
- Why: guessing every action until one works must cost time, so the player diagnoses first.
- On screen: "Wrong action: busy N s" on the admin console (repair buttons greyed) and on the rack, roof panel and extinguisher prompts.
- Code: `game-core/Game.cs` (`Repair`, `WrongActionBusySeconds`), `game-core/PlayerProfile.cs`; text: `dusk-till-dawn/main/BusyText.cs`.

### Warning mails

- A Warning mail says **what is going wrong** and **where it shows** (Monitoring, a floor), never the action that fixes it.
- Example: "Someone is flooding the network. Check the traffic on Monitoring.", not "Turn on the traffic filter."
- The dashboard alerts in §2 follow the same rule: a symptom, never a fix.
- Why: the player diagnoses on the computer first, then walks to the fix. With the Busy rule, guessing costs time.
- Exception: the Fire mail keeps "Grab an extinguisher". The flames already make the fix obvious.
- Code: the `Warning` of each Incident in `game-core/` (`SecurityAttack.cs`, `Fire.cs`, …).

### Rack choice

- **E** at the server room rack opens a choice: **Replace disk**, **Restart cooling**, **Reboot servers**. The game no longer picks the fix for the player.
- Replace disk and Restart cooling are repairs: a choice that fixes nothing while a challenge runs makes the player Busy (see Wrong action).
- Reboot servers is not a repair: it only works after an Auto-shutdown once the room has cooled, and otherwise does nothing and costs nothing (like Eco Mode on the admin console).
- While Busy, E shows the countdown and does not open the choice.
- Code: `dusk-till-dawn/main/disk_repair/DiskRepairSpot.cs`.

## 4. Challenges per night (proposal, to validate with the team)

Our six challenges mapped to the v1 incidents (`docs/game-v1.md` §8):
Security attack = DDoS (#2), Broken hard drive = Failing hard drive (#7),
Overheating server and Fire = Cooling failure → fire (#6), Fake e-mail = Phishing (#8).
Unplugged cable is not in the v1 list. TLS (#1), dependency (#3), bad update (#4) and solar panels (#5) are not built yet.

| Night | Challenges | One new challenge every | Fake e-mails | v1 table (§9) |
|---|---|---|---|---|
| 1 | Broken hard drive, Security attack, Overheating server | 180 s (3 min) | 1 | 1 every ~3 min |
| 2 | Same as night 1 | 150 s | 2 | 1 every ~2.5 min |
| 3 | + Unplugged cable | 120 s | 2 | 1 every ~2 min |
| 4 | + Fire | 90 s | 3 | 1 every ~90 s |
| 5 | Same as night 4 | 75 s | 3 | 1 every ~75 s |

- Fake e-mails come from Phishing mails, not from the spawner (section 5). `docs/game-v1.md` §9 has no Phishing before night 4: to agree with the team.
- A night lasts 10 real minutes, so night 1 gets about 3 challenges and night 5 about 8.
- Fire spawns on its own from night 4; before that it only comes from Electrical faults (`ElectricalFaultRisk`).
- Two spawned challenges overlapping on night 5 is not built: one spawned challenge runs at a time, plus at most one Fake e-mail.
- The game only plays night 1 for now; nights 2 to 5 come with the shop (#64).
- Code: `game-core/NightPlan.cs`.

## 5. Fake e-mail (proposal, to validate with the team)

A Phishing mail starts a Fake e-mail challenge when it arrives, if none is running and the night still allows one
(table in section 4). Reporting the mail fixes it. The other Phishing mails can still be reported, for no points.

| Time since the mail arrived | Stage | SLA drain per in-game minute |
|---|---|---|
| 0 to 60 s | Warning | none |
| 60 s to 4 min | Active | 0.04 % |
| 4 to 5 min | Escalated | 0.08 % (doubled) |
| 5 min | Failed: counts as a failure, −50 points | stops |

- Points: 100, so +150 when reported in the first 2.5 min (fast repair bonus, section 3), +100 after.
  This matches #44 ("+150 in Warning, +100 later") with the one points rule.
- A wrong report (a real mail) still costs −25. The Mail app shows "Reported ✓ +150" or "Not phishing −25".
- Ignored, one Fake e-mail costs about 14 % SLA (216 in-game min × 0.04 + 72 × 0.08), the same as an ignored
  challenge of another kind, plus the −50 failure. Fixed, it gives no SLA back: it drained nothing in Warning.
- Code: `game-core/PhishingEmail.cs`, `game-core/PhishingIncidents.cs`.
