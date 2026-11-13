# Class diagram (object-oriented)

The classes to write in C#. They come from the [game diagram](game-states.md), the [list of challenges](challenges.md) and the [dashboard](dashboard.md).

Read it top to bottom: "The dashboard reads the game", "A game contains players", etc.

## 1. Main classes

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart TD
    Dashboard["<b>Dashboard</b><br/>Shows alerts and points"]
    Game["<b>Game</b> (abstract)<br/>Starts challenges<br/>Says if the game is over"]
    Solo["<b>SoloGame</b><br/>Ends after 3 failures"]
    Duo["<b>DuoGame</b><br/>Ends after the last challenge<br/>Gives the winner"]
    Player["<b>PlayerProfile</b><br/>Name, points, room<br/>Moves, does a challenge"]
    Server["<b>Server</b><br/>Number, state<br/>Breaks down, gets repaired"]
    Challenge["<b>Challenge</b> (abstract)<br/>Alert, room, points, time<br/>Succeeded or failed"]

    Dashboard -->|"reads"| Game
    Game -->|"can be"| Solo
    Game -->|"can be"| Duo
    Game -->|"contains 1 or 2"| Player
    Game -->|"contains 4"| Server
    Game -->|"starts"| Challenge
```

## 2. Challenges

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart TD
    Challenge["<b>Challenge</b> (abstract)"]
    Challenge -->|"can be"| A["SecurityAttack"]
    Challenge -->|"can be"| B["BrokenDisk"]
    Challenge -->|"can be"| C["OverheatingServer"]
    Challenge -->|"can be"| D["Fire"]
    Challenge -->|"can be"| E["UnpluggedCable"]
    Challenge -->|"can be"| F["FakeEmail"]
```

## 3. States of a challenge (state machine)

A challenge is always in **exactly one state**. On every tick, the game moves its challenges forward.

```mermaid
%%{init: {"flowchart": {"curve": "linear"}}}%%
flowchart TD
    Pending["<b>Pending</b><br/>The challenge is ready, not started yet"]
    Running["<b>Running</b><br/>Alert on the dashboard<br/>The timer is running"]
    Succeeded["<b>Succeeded</b><br/>The player earns the points"]
    Failed["<b>Failed</b><br/>The server breaks down"]

    Pending -->|"the game starts the challenge"| Running
    Running -->|"a player does the right action"| Succeeded
    Running -->|"the timer reaches 0"| Failed
```

| State | What happens on each tick |
|---|---|
| Pending | Nothing |
| Running | The time remaining goes down. If it reaches 0, the challenge becomes "Failed". |
| Succeeded / Failed | Nothing. It is over, the challenge does not change anymore. |

In two-player mode it is the same challenge: the first player who does the right action makes it "Succeeded".

## 4. Object-oriented ideas used

| Idea | Where in the diagram |
|---|---|
| **Inheritance** | `SoloGame` and `DuoGame` inherit from `Game`. Each challenge inherits from `Challenge`. |
| **Abstract class** | `Game` and `Challenge`: never created on their own, only their children. |
| **Polymorphism** | "Is the game over?": in solo after 3 failures, in duo after the last challenge. Same question, different answer. |
| **Encapsulation** | A player's points only change through a method, never directly. |
| **Composition** | A `Game` contains its players and its servers. If the game stops, they go away with it. |
| **Separation** | `Dashboard` reads the game but never changes it (rule from the [contract](../contract.md)). |
