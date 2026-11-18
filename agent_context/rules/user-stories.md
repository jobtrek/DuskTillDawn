# Writing user stories

One story = one vertical slice the team can finish and review in one small PR.
Never invent features beyond `docs/contract.md` and `docs/game-v1.md`.

## Default format (use this for most stories)

Plain text, these parts in this order:

- **Depends on #N.** First line, only if it depends on another issue.
- **Goal**: 1–2 sentences. What changes and why.
- **Today** (optional): current state, only if Goal can't say it in one sentence.
- **To do**: short steps. Doc rule first, team validation, then where the code goes.
- **Done when**: one line, the check that proves it's finished (not tests).

No **Tests** section unless the user explicitly asks for tests ([code.md](code.md#tests)).

Example:

```
Depends on #33.

Goal
Write down how points are counted, then code it. Today a challenge always gives fixed points.

To do
- Short rule in docs/player-flow/challenges.md: base points, bonus for a fast repair, penalty for a failure
- Validate the rule with the team
- Code it in game-core/ (a failure removes points from the player)

Done when
A solved challenge gives points by the rule, and a failure removes them.
```

## When a story needs more detail

Use the fields of `.github/ISSUE_TEMPLATE/user-story.yml` (or `task.yml` for a task):
task description, acceptance criteria, technical information, additional notes.
**Skip "Design considerations".**
