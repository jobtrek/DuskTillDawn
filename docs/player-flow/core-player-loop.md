# Player flow

How the game runs from the main menu to the end. Numbers and rules come from [game-v1.md](../game-v1.md). If they change, change them there first.

**v1 is simple mode with one player.** Dashed arrows are later milestones.

## 1. The whole game

```mermaid
flowchart TD
    Menu["Main menu"] --> Mode{"Choose a mode"}
    Mode -->|"Simple mode (v1)"| Start["Start the night<br/>SLA = 100 %"]
    Mode -.->|"Co-op (M3)"| Start
    Mode -.->|"Infinite mode (M2)"| Endless["Endless night<br/>incidents keep speeding up"]

    Start --> Play["Play from 22:00 to 06:00<br/>(10 real minutes)"]
    Play --> Sla{"SLA at 0 %<br/>before 06:00?"}
    Sla -->|"Yes"| Lost["LOSE: company bankrupt<br/>everything resets"]
    Sla -->|"No"| Report["Night report<br/>SLA, incidents, points"]
    Report --> Last{"Was it night 5?"}
    Last -->|"Yes"| Won["WIN the run"]
    Last -->|"No"| Shop["Shop<br/>buy upgrades with points"]
    Shop -->|"Next night, harder"| Start

    Endless -.->|"SLA at 0 %"| Score["Score = hours survived"]
    Lost --> Menu
    Won --> Menu
    Score -.-> Menu
```

## 2. The building

One player, three floors, all on screen in a cut-away view. The player diagnoses on the computer, but can only fix an incident in its room.

```mermaid
flowchart TB
    Roof["Roof (top)<br/>Solar panels, antenna<br/>Incident: dirty panels"]
    Control["Control room (middle)<br/>Desk and computer<br/>Incidents: TLS, DDoS, vulnerable dependency, bad update, phishing"]
    Basement["Basement (bottom)<br/>Servers, cooling, cables<br/>Incidents: cooling failure and fire, failing drive"]
    Roof <-->|"Stairs, 8 s"| Control
    Control <-->|"Stairs, 8 s"| Basement
```

## 3. What the player does during a night

This loop takes about 1 minute and repeats until the night ends.

```mermaid
flowchart LR
    Alert["Alert<br/>mail, dashboard or alarm"] --> Diagnose["Diagnose<br/>on the computer"]
    Diagnose --> Where{"Where is<br/>the fix?"}
    Where -->|"Control room"| Fix["Fix it<br/>only possible in its room"]
    Where -->|"Roof or basement"| Walk["Walk to the right floor<br/>8 s between floors"]
    Walk --> Fix
    Fix --> Reward["Incident resolved<br/>SLA stops dropping, points"]
    Reward --> Alert
```

Other incidents keep coming while the player walks. This is the main pressure.

## 4. The states of an incident

```mermaid
stateDiagram-v2
    [*] --> Dormant
    Dormant --> Warning: Incident starts
    Warning --> Active: Ignored for 60 s
    Active --> Escalated: Ignored for 3 min
    Warning --> Resolved: Fixed, +50 % points
    Active --> Resolved: Fixed
    Escalated --> Resolved: Fixed
    Resolved --> [*]
```

| State | SLA | Player sees |
|---|---|---|
| Warning | No loss | Mail or dashboard hint |
| Active | Drops, at the rate in the incident's data file | Alarm, red on the dashboard |
| Escalated | Drops twice as fast, may start a cascade | Louder alarm, extra effects |
| Resolved | Stops dropping | Green, points |

## 5. Cascades

v1 has exactly three. Fixing the root cause beats fixing the symptoms.

```mermaid
flowchart LR
    Dirty["Panels dirty or damaged"] --> Charge["Battery charges slower"]
    Charge --> Power["Power drops"]
    Power --> Slow["Servers slow down"]

    Cooling["Cooling fails"] --> Heat["Heat rises"]
    Heat --> Fire["Fire"]

    DDoS["Traffic spike (DDoS)"] --> Load["Load rises"]
    Load --> HeatUp["Heat rises"]
    Load --> Draw["Power draw rises"]
```

## 6. Gains and losses

| Gains | Losses |
|---|---|
| Points for each resolved incident | SLA drops for each active incident |
| +50 % points if fixed during Warning | SLA drops twice as fast when escalated |
| +200 points for surviving the night | Extra -1 % per in-game minute when servers are down |
| +1 point per SLA % left | SLA at 0 %: the run ends and everything resets |
| Upgrades stay for the whole run | |

## 7. How the game code connects to the 3D models

The rules know nothing about the models. Scene nodes read the game state and show the matching model, so a model can replace a grey box without changing the rules.

```mermaid
flowchart LR
    Input["Keyboard input"] --> Player["Player.TryFix(incident)"]
    Data["Incident data files"] --> State["GameState<br/>time, SLA, incidents, values"]
    Player --> State
    State -->|"state changed"| Views["Scene nodes<br/>show the state, never own it"]
    Models["Blender models<br/>exported as .glb"] --> Views
    Views --> Screen["Screen"]
```

Each State of an Element is its own model, swapped in place:

| Game state | Model shown |
|---|---|
| Panel clean, dirty or damaged | `SolarPanel_Clean`, `SolarPanel_Dirty`, `SolarPanel_Damaged` |
| Server on, hot, off or failed | `Server1U_On`, `Server1U_Hot`, `Server1U_Off`, `Server1U_Failed` |
| Cooling on, off or broken | `ACUnit_On`, `ACUnit_Off`, `ACUnit_Broken` |
| Drive healthy or failed | `HardDrive_Healthy`, `HardDrive_Failed` |

## 8. Multiplayer (M3, not in v1)

Two players defend the same building. The game flow stays the same, but some incidents need **both players to act at once**. Example from [multiplayer.md](../ideas/multiplayer.md):

```mermaid
sequenceDiagram
    participant Game
    participant P1 as Player 1 (roof)
    participant P2 as Player 2 (control room)
    Game->>P1: DDoS alert
    Game->>P2: DDoS alert
    P1->>Game: Align the radio antenna
    P2->>Game: Enable traffic filtering
    Note over Game: Both done within 30 s?
    Game-->>P1: Incident resolved
    Game-->>P2: Incident resolved
```