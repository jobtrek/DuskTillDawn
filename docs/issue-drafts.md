# Issue drafts (v1 backlog)

Drafts to discuss with the team before posting them on GitHub. Format:
[user-stories.md](../agent_context/rules/user-stories.md). Numbers come from
[game-v1.md](game-v1.md) unless a line says "proposal".

Vocabulary: these drafts say "incident" like game-v1.md. The code still says
`Challenge` until PR #50 merges.

---

## 1. Each night spawns more incidents, faster

Depends on #54 and #64 (it adds the night number).

**Goal**
Nights 1 to 5 get harder: each night unlocks more incidents and spawns them faster.
Surviving night 5 wins the run. See game-v1.md §9.

**Today**
The spawner (#54) launches one incident every 20 s from a fixed list, whatever the night.

**To do**
- Put the night table from game-v1.md §9 in one place in `game-core/` (incidents per night, spawn interval)
- The spawner reads its interval and incident list from the current night
- Night 5 lets two incidents overlap
- Winning night 5 ends the run with a win
- Validate the numbers with the team

**Done when**
Night 1 only spawns incidents 1 and 5, every ~3 min. Night 5 spawns all 8, every ~75 s, and winning it ends the run.

---

## 2. Shop between nights with counter-upgrades (posted as #64)

**Goal**
Between two nights the player spends points on counter-upgrades that remove or shorten an
incident. See game-v1.md §8 (counter-upgrade column) and §9 (Points and shop).

**Today**
There is only one night: at the next 20:00 the HUD shows "Day 1 survived" and the game pauses
(`Main.cs:291`). There is no night number and nothing to buy.

**To do**
- Short rule in game-v1.md §9: price of each upgrade (300 to 1500 points) and what it changes on its incident
- Validate prices with the team. Proposal: the shop opens at 08:00, while the SLA is frozen, and a
  "Start next night" button skips to 20:00
- Night number on `GameState` (starts at 1). "Start next night" adds 1, resets the SLA to 100 and empties the inbox, instead of pausing the game
- End-of-night points in `game-core/`: +200 for surviving, +1 per SLA % left
- Upgrade list and purchase in `game-core/`: buying costs points, an upgrade lasts the whole run, losing resets all upgrades
- The spawner skips or shortens an incident whose upgrade is bought
- Shop screen in Godot ([GUI guide](../agent_context/gui/README.md)), with one line on why the upgrade works

**Done when**
After night 1 the player buys Auto-renewal, starts night 2, and the TLS incident never spawns again during that run.

---

## 3. The three cascades move Power, Heat and Traffic

Depends on #55 (it adds the Fire incident).

**Goal**
Code the three cascades from game-v1.md §5 so the player sees that one problem causes another.

**Today**
`GameState` has Power, Heat, Traffic and Disk fields, but nothing changes them.

**To do**
- Short rule in game-v1.md §5: rate of change per in-game minute for each value
- Validate the rates with the team
- In `GameState.Tick` (`game-core/`):
  - dirty panels: battery charges slower, so Power drops; at 0 % servers go offline (−1 % SLA per in-game minute)
  - cooling failure: Heat rises; at 95 °C a Fire incident starts
  - DDoS: Traffic goes above 100 %, so Heat and Power draw rise
- The dashboard shows the four values (check it already reads them)

**Done when**
Leaving a cooling failure unfixed makes Heat climb past 95 °C and starts a fire on its own.

---

## 4. The helper picks the right hint (game-core)

**Goal**
The help app knows what to say: a hint for the incident the player is stuck on, or a
tutorial line on night 1. Spec: [helper_app_spec.md](specs/helper_app_spec.md).

**Today**
The desktop icon sends `HelpPressed`, but nothing listens to it. Incidents have no hint text.

**To do**
- Answer the 4 open questions in helper_app_spec.md with the team. Proposal:
  - Name and look: pick a name and a 32×32 face (not "ChatGPT")
  - A dot on the desktop icon when a hint is ready; the text only shows inside the app
  - "Stuck for 60 s" counts from Active, so Warning stays the player's own chance to spot it
  - Hints are free in v1
- Add `Hint` and `TutorialText` to each incident
- In `game-core/`: a method that returns the text to show (night 1 new incident → tutorial;
  oldest incident Active for 60 s or more → its hint; otherwise an idle line)
- Remember which hints were acknowledged, so "Got it" hides that hint

**Done when**
An incident left Active for 60 s makes the helper return its hint, and "Got it" stops showing it.

---

## 5. Help app window on the computer

Depends on draft 4.

**Goal**
Clicking the help icon opens the helper window from helper_app_spec.md and shows the hint from draft 4.

**To do**
- Rename the "ChatGPT" icon label and tooltip in `computer_menu.tscn`
- Build the window with `/create-ui` (portrait, speech bubble, footer with the incident and "Got it")
- Listen to `HelpPressed` in `ComputerMenu.cs` to open it; [X] and Escape close it
- Show a dot on the help icon while a hint is ready

**Done when**
With an incident stuck for 60 s, the help icon shows a dot, and opening the app shows that incident's hint.

---

## 6. Buy a server stage in the shop

Depends on #64.

**Goal**
The player buys the next server stage between nights, and the server room changes.

**To do**
- `game-core/`: server stage on `GameState` (starts at 1), buying it costs points, kept for the run, reset on loss
- Price of each stage in points and its effect on Traffic capacity, Heat and Power draw: short rule in game-v1.md §9 "Server stages", validated with the team
- Each stage changes Traffic capacity, Heat and Power draw by those numbers
- Shop shows the next stage, its price and its effect. At most one stage per shop visit
- Server room scene swaps its models for the stage (grey-box is fine; models follow [assets README](../agent_context/assets/README.md))

**Done when**
After buying stage 2, the next night handles a DDoS without dropping requests, and the server room shows the new hardware.
