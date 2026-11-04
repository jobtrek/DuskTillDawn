# Dawn to Dusk: contract (v0.1)

The agreement between the three of us on what the first version of the game is. If something here changes, all 3 of us agree first, then we edit this file.

Status: **draft**. Sections marked `TBD` are decided in the contract meeting.

---

## 1. Game in one sentence

You are the only sysadmin on the night shift. Keep the company's servers alive until 06:00.

Details: [game-v1.md](game-v1.md).

## 2. Technical decisions

| Decision | Choice | Notes |
|---|---|---|
| Engine | Godot 4 (.NET build) | |
| Language | C# (OOP) | The whole project, no GDScript |
| Dimension | **3D** | Real 3D scenes and models, not 2D sprites |
| Camera | **Isometric** | Fixed angle |
| Art style | Voxel (hard-edged blocks) | Models exported as `.glb` |
| Platform | Desktop | |
| Methodology | Scrum, 3 day sprints, igor has CIE for the next 4 weeks. | |

### 2.1 What "3D + isometric" means for us

- The world is built from 3D models. The player character walks in 3D space.
- The camera uses **orthogonal projection** (no perspective), so objects keep the same size at any distance.
- The camera angle is **fixed**. The player cannot rotate or zoom freely.
- One room (floor) is shown at a time. Moving between floors switches the room, it does not scroll the world.
- Everything the player must click or reach stays visible from this angle. No important prop hides behind a wall. If it does, the room layout changes, not the camera.

| Camera value | Starting value | Notes |
|---|---|---|
| Projection | Orthogonal | |
| Horizontal angle | 45° | Tunable |
| Vertical angle | about 35° (true isometric) or 30° (game isometric) | `TBD`: pick one during the grey-box |
| Rotation by player | None in v1 | Revisit only if props get hidden |

### 2.2 Consequences

- Voxel models must look good from one angle. Hidden faces can be cheaper.
- Model detail has a budget, because Godot has to render several rooms. The benchmark of 3D-model generation takes this into account.
- Merge conflicts in `.tscn` files are likely: **one scene, one owner** at a time.

## 3. Player mode: single player first

**v1 is single player only.**

- One player controls one character.
- All incidents are designed so that **one person can solve them**.
- No networking code in v1. No multiplayer state sync.
- Co-op (2 players) comes back in milestone M3. See the parked list in [game-v1.md](game-v1.md#11-parked-ideas-not-in-v1).

### 3.1 Rules that keep co-op possible later

Single player does not mean we ignore co-op. Cheap habits now save a rewrite later:

1. All game state lives in one place (a `GameState` class), never scattered in scene scripts.
2. Player actions go through methods like `player.TryFix(incident)`, not directly from input code to game state.
3. Scene nodes show the state, they do not own it.

## 4. v1 scope

Taken from [game-v1.md](game-v1.md). Confirm in the meeting.

| Item | v1 |
|---|---|
| Players | 1 |
| Floors | 3 (roof, control room, basement) |
| Nights | 5 |
| Incidents | 8 |
| Computer apps | Dashboard, mail, AI helper |
| Shop | Between nights, counter-upgrades only |
| Modes | Simple mode only (infinite mode later) |

## 5. Pillars

Every feature supports at least one. If not, it waits.

1. **Pressure, not panic.**
2. **Real incidents, simplified.**
3. **Everything is connected.**

## 6. Open decisions (`TBD`)

To settle in the contract meeting. Proposals are in `initial_organisation.md`.

| # | Question | Proposal |
|---|---|---|
| 1 | Vertical slice size (1 night / 6 incidents vs 5 nights / 8 incidents) | Night 1, 4 incidents (#1, #5, #6, #7) |
| 2 | Meaning of "2 player mechanic" (see [ideas/multiplayer.md](ideas/multiplayer.md)) | Co-op only, versus parked |
| 3 | Day phase (sleep / work cycle) | Shop only in v1 |
| 4 | Lose condition | SLA = 0 only |
| 5 | Roles (Dev 1/2/3, product owner, first scrum master) | Assign by interest |
| 6 | Audience and client | Confirm with the client |
| 7 | Isometric vertical angle (30° or about 35°) | Pick in the grey-box |
