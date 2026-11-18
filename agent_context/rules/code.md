# Code rules

Hard rules. CI enforces some of them; the rest are on you.

## Language and style

- **C# only.** No GDScript (the MCP bridge addon is the only exception).
- **Always use braces** `{}` on `if`, `else`, `for`, `foreach`, `while`, even for one line.
- Code must pass `dotnet format --verify-no-changes` (`.editorconfig`).
- Follow the C# coding conventions (Microsoft) and Godot best practices.
- When writing comments do not include ///<summary>x</summary>, and when writing comments make sure it explain the domain and not what the code does (unless very complex logic or if the user asked you too.) if none of the above is applicable don't write any comments.

## Where code goes

- **Game state and rules live in `game-core`** (`GameState` and friends), never in a
  Godot node. Nodes show state and forward player actions.
- Player actions go through methods (e.g. `player.TryFix(incident)`), not from
  input code straight into state.
- UI: the App emits signals, a Controller decides (see
  [architecture/godot-project.md](../architecture/godot-project.md)).

## Scenes

- **One owner per `.tscn`.** Do not edit a scene the user did not ask you to touch;
  `.tscn` merge conflicts are expensive.

## Tests

- When implementing a feature or mechanic, **do not write tests unless asked.**
- When asked, write the tests **first**, then the code (never tests after the code).
- Existing tests (`game-core.tests`) must keep passing: run `dotnet test`.

## Before you say done

`dotnet build` and `dotnet test` pass, `dotnet format --verify-no-changes` is clean.
Library or engine API involved: check current docs first (`find-docs` skill).
