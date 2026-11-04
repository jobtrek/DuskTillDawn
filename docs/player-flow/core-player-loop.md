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

## 2. What the player does during a night

This loop takes about 1 minute and repeats until the night ends.

```mermaid
flowchart LR
    Alert["Alert<br/>mail, dashboard or alarm"] --> Diagnose["Diagnose<br/>on the computer"]
    Diagnose --> Walk["Walk to the right floor<br/>8 s between floors"]
    Walk --> Fix["Fix it<br/>only possible in its room"]
    Fix --> Reward["Incident resolved<br/>SLA stops dropping, points"]
    Reward --> Alert
```

Other incidents keep coming while the player walks. This is the main pressure.

## 3. The states of an incident

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

## 4. Gains and losses

| Gains | Losses |
|---|---|
| Points for each resolved incident | SLA drops for each active incident |
| +50 % points if fixed during Warning | SLA drops twice as fast when escalated |
| +200 points for surviving the night | Extra -1 % per in-game minute when servers are down |
| +1 point per SLA % left | SLA at 0 %: the run ends and everything resets |
| Upgrades stay for the whole run | |

## 5. Multiplayer (M3, not in v1)

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