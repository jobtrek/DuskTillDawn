---
name: docs-coherence
description: Audit a repo's docs for contradictions, stale or wrong claims, and opinions stated as facts, against each other and against the code. Read-only report.
disable-model-invocation: true
argument-hint: "[docs path | --since <git ref>] (default: all docs)"
allowed-tools: Read, Grep, Glob, Write, AskUserQuestion, Bash(rg:*), Bash(sgrep:*), Bash(git log:*), Bash(git diff:*), Bash(git ls-files:*), Bash(sort:*), Bash(uniq:*), Bash(cut:*), Bash(wc:*), Bash(test:*), Bash(curl -sIL:*)
---

Audit the repo's documentation as a set of **claims**: statements that something is true, decided, or built. Find every claim that conflicts with another claim, with the code, or with the environment. Change nothing; the output is a report.

**Read-only.** The only file you write is the scratch claim table outside the repo (`/tmp/docs-coherence-claims.tsv`, or the session scratchpad). Every command is non-mutating: no installs, builds, formatters, or writes in the repo. Never dispatch sub-agents; everything runs in this session.

## Search tools

- **`rg`** when you know the string: paths, filenames, scripts, env vars, ports, config keys, versions, ADR ids, link targets, anchors, every mention of a term, and claims against code (`rg -n 'Symbol' src/`). List docs with `rg --files -g` rather than `find`/`ls`.
- **`sgrep`** when you know the idea: same topic in different words ("auth uses sessions" vs "login is JWT-based"), one term with two meanings, code that implements or contradicts a behaviour when you don't know the symbol. Query in the doc's own language.
- A semantic hit is a lead, not evidence. Read the lines before it becomes a finding, and cite `file:line` from the read, not the hit list.
- If `rg` or `sgrep` is missing or fails (e.g. sgrep with no embedding server), say so once at the top of the report, fall back to the built-in Grep tool plus the topic column of the claim table, and mark cross-doc findings that relied on it "likely". Don't install anything to fix it.
- Never use flags that write into the repo (e.g. `sgrep --debug-log`). The sgrep index lives in `~/.sgrep`, outside the repo.

## Asking the user

Use **AskUserQuestion** for every question. Never ask in plain text, and never silently guess when a step says to ask.

- Ask only what the repo cannot answer. A command, a date, a status line or an explicit "supersedes" settles it without asking.
- Up to 4 questions per call, 2–4 options each, `header` under 12 characters. Every option is a concrete answer. Put your best guess first with "(Recommended)".
- Put the evidence in the question (`file:line` and both claims, shortened), so the user can answer without opening files.
- "Other" free text is authoritative.
- **No AskUserQuestion** (scheduled or headless run): mark each would-be question "unclear" and continue.

## 0. Confirm scope (only when needed)

Default set: `docs/**`, root and nested package `README*`, `AGENTS`, `CLAUDE`, `CONTEXT`, `GLOSSARY`, `CHANGELOG`, every `adr/` folder, OpenAPI specs (`openapi*.{yml,yaml,json}`), and `.claude/` skills and rules. A path argument replaces it.

**`--since <ref>`**: audit the docs from `git diff --name-only <ref>`, plus docs that link to them (`rg` on their filenames) or share their topics (`sgrep` on their headings).

If there is no argument **and** the set is ambiguous (several `docs/`-like folders, a monorepo, more than ~40 docs), ask once:

- **Scope** (multiSelect): the candidate folders/packages.

## 1. Map the doc set

List every doc with its status (decision, proposal, reference, analysis). If `docs/.authority.md` exists, load its saved decisions first and never re-ask them.

Build the **authority order** from what the repo states only: "fait foi", "supersedes", "source of truth", `Status: accepted / superseded / proposal`. Then dates: the doc's date header, else `git log -1 --format=%cs -- <file>`, never the file's mtime. Then ADRs over analyses, decisions over proposals. Pairs this leaves unordered are asked in step 5, once their topics are known.

Done when: every doc has a status and every explicit or dated precedence is recorded.

## 2. Check references mechanically

Script it with `rg` and a shell loop; don't verify by hand. Check that:

- every path, file, script, package, skill and relative link exists, and every `#anchor` matches a heading;
- every CLI command or flag in a code block exists (`package.json` scripts, `Makefile` targets, `--help` of a read-only CLI);
- every env var, port and config key a doc names appears in config or code;
- every pinned version matches the manifest or lock file;
- every cited ADR exists, and its status matches how it is cited;
- external URLs answer (`curl -sIL`, HEAD only).

No questions here: these are facts, not judgement.

Done when: every reference has been checked, not sampled.

## 3. Extract and cross-check claims

Read docs in batches by topic. As you read each doc, append its load-bearing claims (decisions, term definitions, versions, behaviour, status of work) to the scratch table, one per line:

`topic | kind | claim | file:line | quote`

Pick topics by meaning, not wording, so two phrasings of one question share a topic. Then cross-check in two passes:

1. **Exact:** `sort` the table by topic and `rg` it for shared terms, versions and identifiers. Same topic or key with different values is a candidate. Run the same identifiers against the code.
2. **Semantic:** per topic, `sgrep` with the claim text and read the top hits across the other docs and the code. This catches same meaning in different words, one term with two meanings, a reversed decision with no "superseded" mark, and a proposal that other docs treat as decided (or the reverse).

Done when: every topic in the table has had both passes.

## 4. Classify

| Type | Meaning |
| --- | --- |
| **Contradiction** | Two docs give different answers to the same question. |
| **Wrong** | The code, a manifest or a command disproves the doc. |
| **Stale** | True once: an unmarked superseded ADR, a renamed file, an old version. |
| **Dangling** | Points at something that does not exist. |
| **Proposal-as-decided** | A proposal or open question stated or used as settled. |
| **Opinion-as-fact** | An opinion stated as a verified fact in a doc ranked authoritative. |

Opinions inside a proposal or analysis are its job, not a finding.

**Severity:** High if following the doc builds the wrong thing; Medium if it wastes time; Low if it only misleads a reader.

**Confidence:** *certain* only when a command or the code proves it, or you read both sides. *Likely* when it rests on an sgrep lead or the Grep fallback.

**Group by root cause.** One renamed file referenced 12 times is one finding listing its locations, found with a single `rg`.

Every finding carries evidence: both quotes with `file:line`, or one quote with the command and its output. A finding without evidence is dropped.

## 5. Resolve open conflicts

Ask about the findings with no winner, plus the authority pairs left open in step 1. Order them by severity, at most **3 AskUserQuestion calls** of 4 questions each:

- **Conflict**: "`README.md:42` says auth uses sessions; `docs/api.md:17` says JWT. The code has no auth yet. Which is the intended design?" Options: "Sessions (README)", "JWT (api.md)", "Undecided: mark both as open".

Record each answer as that finding's winner. Anything skipped, beyond the cap, or answered "undecided" stays **unclear**.

## 6. Report

Report in chat unless the user asked for a file. Template:

```
**Docs audit: N findings (H High, M Medium, L Low).** <tool fallback note, if any>

| Type | High | Med | Low |   (one row per type with findings)

| # | Type | Sev | Conf | Where | Evidence | Wins | Fix |   (highest severity first)

**Open decisions:** the unclear findings, one line each.
**Gate candidates:** findings a script could catch on every commit (dangling paths, anchors, version drift, ADR status), so the next drift fails CI.
```

"Wins" comes from step 1 or the user's answer in step 5 (mark those "user"). With zero findings: `**Docs audit: 0 findings** across N docs, R references checked.` plus the gate candidates.

## 7. Next step

The skill stays read-only. After the report, ask one final question:

- **Next step**: "Fix High findings", "Write the gate scripts", "Save authority decisions to `docs/.authority.md`", "Done".

Act only on the answer; this skill never edits files before that.
