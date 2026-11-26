# Issue drafts: game-core organisation

Drafts to discuss with the team before posting them on GitHub. Format:
[user-stories.md](../agent_context/rules/user-stories.md). Vocabulary: [CONTEXT.md](../CONTEXT.md).

---

## 1. Group game-core by feature and use the glossary names

**Goal**
Make every game-core file easy to find and named like the glossary. Today 33 files sit flat
in `game-core/`, and the code says "Challenge" where the glossary says "Incident".

**To do**
- Validate the folder list with the team: `Incidents/`, `Power/`, `Mail/`, `Weather/`, `Servers/`, `Night/`, `Building/`
- Move each file into its folder; namespaces follow folders (`DuskTillDawn.Core.Incidents`)
- One type per file: move `Room`, `PlayerAction` out of `Challenge.cs` and `WarningText` out of `Mail.cs`
- Rename `Challenge` → `Incident` (with `ChallengeSpawner`, `ChallengeState`, `Mail.ChallengeId`)
- Rename `StoreInstancesOfIncidents` → `IncidentPool` and `ServerStatus` → `RoomPowerState`
- Remove `DuoGame`; `Game` stays as the Incident engine, `GameState` stays the root
- Update the maps that name moved files (`where-to-change.md`, `overview.md`, `game-core.md`)

**Done when**
No file sits at the root of `game-core/` and no type name contains "Challenge".

---

## 2. One access-level rule for game-core

Depends on #(issue 1).

**Goal**
Every game-core class exposes its state the same way, so Godot nodes can read state but only change it through methods.

**To do**
- Write the rule in `agent_context/rules/code.md` and validate it with the team:
  1. State the UI reads: `public X { get; private set; }`, changed only through a named method
  2. Fixed data (a Mail, a WarningText): `record` or `required init`
  3. `internal set` only when another game-core class must write it, with a reason
  4. Lists: `IReadOnlyList<T>` over a private `List<T>`
  5. No public fields
- Apply it to every class in `game-core/`

**Done when**
A teammate can review any game-core class against the five points and finds no exception without a reason.

---

## 3. Every Incident goes through the Escalation stages

Depends on #(issue 1).

**Goal**
Every Incident follows Dormant → Warning → Active → Escalated → Resolved (game-v1.md §7).
Today only the Fake e-mail has stages; the others just succeed or fail on a timer.

**To do**
- Validate with the team: an unfixed Incident stays Escalated until Resolved or the night ends (no "failed")
- `Incident` holds the stage; defaults Warning 60 s and Active 3 min, each Incident may override them
- Warning: no damage, a fix gives +50 % points. Active: SLA drains. Escalated: drain doubles
- An Electrical fault starts a Fire when it reaches Escalated (today: when it fails)
- The Failure penalty applies when an Incident reaches Escalated; `Failures` becomes `Escalations`
- A night is lost only when the SLA reaches 0 (remove the 3-failures loss in `SoloGame`)
- The spawner waits for an Incident to be Resolved before it launches another of that kind
- Update `agent_context/game/rules-v1.md` (timings are defaults per Incident)
- Ask the team to update `docs/game-v1.md` §7 (timings) and line 80 ("incidents failed" → escalations)

**Done when**
An ignored Incident goes Warning → Active → Escalated, keeps draining until fixed, and the night report counts Escalations.
