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

## Keep docs in sync

1. New or moved feature, file, folder, system or term: in the same change, update every
   map that names it (`where-to-change.md`, `overview.md`, `README.md`, `CONTEXT.md`).
2. A file would pass 40 lines: split the topic into a new file, index it in `README.md`.
3. Human-owned `docs/` files are stale: tell the user, never edit them.
4. Docs and code disagree: tell the user right away, both claims with `file:line` and which
   wins. Never silently pick one or record it in a new file. Suggest `/docs-coherence`
   for a full audit (`~/.claude/skills/docs-coherence/SKILL.md` explains the method).
