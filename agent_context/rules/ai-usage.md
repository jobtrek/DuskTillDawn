# AI usage rules

The team's rules for AI (source of truth; `docs/team-rules.md` links here).
The team must understand everything an agent produces.

## Humans own these, agents only critique

- `docs/contract.md`, ADRs in `docs/adr/`: written by hand. Agents never write or edit
  them unless asked; they answer "what's contradictory or missing?". An ADR an agent
  drafts must be understood and agreed on by the team.
- PR titles and descriptions: written by a human. Do not draft them unless asked.
- Learning code stays human: Godot and C# fundamentals are written by the team.
  AI is for boring or verifiable work: boilerplate, test scaffolding, benchmark scripts.
- User stories: follow [user-stories.md](user-stories.md).

## Keep output small

- One AI answer: max 30 lines, no filler. One `agent_context/` file: max 40 lines.
- Docs: short, plain English. No duplicated decisions. Small PRs, no giant diffs.
- Anything AI-generated that goes into the repo gets a pair review before merge.

## Explain-it-back

- Nobody merges what they can't explain in one sentence: write so a teammate can.
- When grilled, the human restates the question; push back when something
  contradicts the contract or the code.

## Scope

Build only v1 (`docs/game-v1.md`). `docs/ideas/` is parked: do not build it.

## Docs and code that disagree

1. Tell the user right away: both claims with `file:line`, and which one you think wins.
2. Do not record it in a new file and do not silently pick one.
3. Suggest a full audit with `/docs-coherence`
   (`~/.claude/skills/docs-coherence/SKILL.md` explains the method).
