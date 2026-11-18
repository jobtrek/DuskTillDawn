---
name: game-pr-review
description: Review a teammate's GitHub PR for technical best practices and docs, then post short inline comments on the exact file/line.
disable-model-invocation: true
---

# game-pr-review

Usage: `/game-pr-review <PR number or URL>`. No argument → ask for the PR number.

## 1. Load the PR

```bash
gh pr view <N> --json number,title,body,author,headRefOid,baseRefName,files
gh pr diff <N>
gh pr checks <N>
```

Read every changed file in full at the PR head (`gh api repos/{owner}/{repo}/contents/<path>?ref=<headRefOid>`), not just the hunks.

Read every issue the PR names (body, commits, code comments like `#35`): `gh issue view <N>`. Its to-do list and "done when" are part of the yardstick in step 2. No linked issue → that is a finding.

## 2. Review

Read the rules before judging; they are the yardstick, not your taste. Read them as they are on the PR's base branch (`git fetch origin <baseRefName>`, then `git show origin/<baseRefName>:<path>`), not your local checkout: a rule the PR author cannot see is not a finding. A path below missing on the base → use the base's own docs (its `CLAUDE.md` says where they live).

- **Technical**: `agent_context/rules/code.md`, `agent_context/rules/git.md`, plus the `architecture/` file for the area touched (`agent_context/README.md` indexes them). Check C# conventions, Godot best practices, logic in `game-core` vs nodes, braces, comments, scene ownership, committed generated files/secrets, failing CI checks.
- **Docs**: `agent_context/rules/ai-usage.md` ("Keep docs in sync"). For every changed feature, file, folder or term: is each map that names it updated (`where-to-change.md`, `overview.md`, `README.md`, `CONTEXT.md`)? Does any changed doc contradict the code, another doc, `docs/contract.md` or `docs/game-v1.md`? Is an `agent_context/` file over 40 lines? Did the PR edit human-owned `docs/` without saying why?

- **Issue**: every to-do item and the "done when" of each linked issue is met, or the PR says why not. A missing item is a conflict with `issue #<N>` (quote its line).

Done when every changed file has been checked against all three lists. A **conflict** is two sources that disagree (doc vs code, doc vs doc): open both and quote the exact line of each.

## 3. Write findings

One finding = one comment, pinned to the exact line it's about. Body format, nothing else:

~~~
**<Short title>**

**Conflicts:** `<other/file.md:line>`

<One or two plain sentences: what is wrong and what to do.>

<details>
<summary>Prompt for an agent</summary>

```text
In `<file>:<line>`, <what to change>.
<Why, in a few words>.
Done when <checkable condition>.
```

</details>

<details>
<summary>Code to fix it yourself</summary>

```<lang>
// <file>
<the fixed code, only the lines that change plus 1-2 lines of context>
```

</details>
~~~

- Keep the empty line between each part, also around `<summary>` and the code fences: GitHub joins lines with no gap into one paragraph and won't render markdown inside `<details>` without it.
- The `Conflicts` line appears only when the issue contradicts another file.
- One sentence per line in every prompt: a code block never wraps, so one long line scrolls off screen.
- The prompt is self-contained: an agent with no other context can apply it from the file path, line and done condition alone.
- The code block appears only when the fix is under 20 lines in one file. Bigger or multi-file fixes get the prompt only.
- The snippet must be correct code at the PR head: read the surrounding file before writing it.

Skip nits a linter catches; report real issues only. Zero findings is a valid result.

The review `body` (the global comment) holds PR-level findings (title, `Conflicts`, description only: no per-finding `<details>`), then ends with every prompt combined, numbered in the same order as the findings:

~~~
<details>
<summary>Prompt to fix all issues</summary>

```text
Fix these issues in PR #<N>:

1. <prompt of finding 1, line 1>
   <line 2, indented 3 spaces>
   Done when <condition>.
2. <prompt of finding 2>
```

</details>
~~~

## 4. Confirm, then post

Show the user the list (`file:line` + body). Post only after they approve. Then post one review, event `COMMENT`:

```bash
gh api repos/{owner}/{repo}/pulls/<N>/reviews --input review.json
# review.json: {"commit_id":"<headRefOid>","event":"COMMENT","body":"","comments":[{"path":"...","line":12,"side":"RIGHT","body":"..."}]}
```

A finding about the PR itself (missing issue link, empty description) goes in the review `body`, same format. A line comment must target a line inside the diff. When the issue is on an unchanged line or the whole file (e.g. a map that was never updated), post a file-level comment instead:

```bash
gh api repos/{owner}/{repo}/pulls/<N>/comments -f commit_id=<headRefOid> -f path=<path> -f subject_type=file -f body="..."
```

Write `review.json` in the scratchpad. Finish by printing the review URL and the count of comments posted.
