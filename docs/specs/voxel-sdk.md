# Voxel SDK: generate Element builds from Spec + Layout

Triage: `ready-for-agent`
Related: `docs/adr/0001-generate-build-py-from-spec-and-layout.md`

## Problem Statement

Every Element's `build.py` is about 570 lines of copied voxel engine plus 20 to 55 lines of hand-written `box`/`tile` calls with voxel coordinates computed by the agent. Across ~65 Elements this adds up to over 25k lines in the repo. The agent has to do cm-to-voxel and relative-position math by hand, loses context between the Spec and the code even when the Spec is detailed, and only discovers mistakes (floating Parts, wrong height, same-colour neighbours, unknown palette keys) after running in Blender.

## Solution

An SDK that turns an Element's Spec plus a new per-Element Layout into a validated voxel model and a generated, self-contained `build.py`. The agent only writes the Layout: where each Part sits relative to other Parts. The SDK converts cm to voxels, resolves placement, validates everything in plain Python (no Blender), and shows an ASCII slice preview so the agent can self-check. The agent never reads engine code. Generated `build.py` still runs by pressing Alt+P in Blender.

## User Stories

1. As an agent building an Element, I want to write only a Layout of relative Part placements, so that I do not do cm-to-voxel arithmetic.
2. As an agent, I want Part sizes and colour keys read from the Spec, so that I never restate them and they cannot drift.
3. As an agent, I want to place a Part with relations like "on neck.top, centered, front-flush", so that placement reads like the design intent.
4. As an agent, I want unknown anchors or Part names to fail loudly with the offending name, so that typos do not produce silent wrong geometry.
5. As an agent, I want unknown palette keys to fail loudly, so that I do not get default or missing materials.
6. As an agent, I want to repeat a Part in a grid (vent slots, buttons, screws) with a count and spacing, so that tertiary details are one line.
7. As an agent, I want circle, disc, ring and hollow Parts, so that mugs, clocks, pipes and dishes are expressible.
8. As an agent, I want a `raw` block for voxel calls the Layout cannot express, so that I am never blocked by the DSL.
9. As an agent, I want `raw` blocks validated by the same checks as other Parts, so that the escape hatch does not bypass quality.
10. As a maintainer, I want the number of `raw` blocks reported, so that I can see where the DSL falls short.
11. As an agent, I want the model size checked against the Spec's size and voxel counts within 1 voxel, so that wrong-height models fail before Blender.
12. As an agent, I want floating Parts detected (parts not connected to the rest), so that I fix them before building.
13. As an agent, I want adjacent Parts with the same colour key flagged, so that Parts stay visually distinct.
14. As an agent, I want an ASCII slice preview of the voxel grid, so that I can check shape without opening Blender.
15. As an agent, I want a per-colour voxel count and bounds in the report, so that I can compare against the Spec cheaply.
16. As an agent, I want one `check` command that validates and prints the preview, so that my loop is one call.
17. As an agent, I want one `build` command that generates `build.py`, so that I never run the bundler by hand.
18. As an agent, I want to read at most the Spec, the palette keys and a short SDK reference, so that my context stays small.
19. As a designer, I want the generated `build.py` to remain self-contained, so that I can open it in Blender's Scripting tab and press Alt+P.
20. As a designer, I want generated `build.py` to carry the Spec as a header comment as today, so that intent travels with the file.
21. As a maintainer, I want `build.py` treated as generated output and never edited by hand, so that Layout is the single authored artifact.
22. As a maintainer, I want the 3-model proof (Monitor, Mug, Server1U) to match existing builds within 1 voxel, so that the SDK is trusted before wider migration.
23. As a maintainer, I want Elements with States or Pieces to keep hand-authored builds in v1, so that v1 scope stays small.
24. As a maintainer, I want the Layout's Part names to agree with the Spec's Parts list, so that Spec and Layout cannot disagree.
25. As a maintainer, I want the Spec's 30-line cap and `build_index.py --check` to keep working, so that the existing pipeline is unaffected.
26. As a maintainer, I want the SDK uv-managed under `game-assets/sdk/`, so that setup and dependencies are reproducible.
27. As a maintainer, I want Grid S/F handling (1/64 m vs 1/128 m) from the Spec, so that cm-to-voxel conversion is correct per Element.
28. As a reviewer, I want diffs for a changed Element to show only a small Layout change, so that review is cheap.

## Implementation Decisions

- The SDK is a pure-Python uv package under `game-assets/sdk/`. It does not import Blender.
- Inputs per Element: the existing Spec (Parts, sizes, colour keys, grid, voxels) and a new `layout.toml` (the **Layout**). Terms follow `CONTEXT.md`: Element, Spec, Part, Layout, Grid, Seam.
- The Layout holds only placement and repeat information. Part names, sizes and colour keys come from the Spec; the SDK checks the Layout and the Spec agree on Part names.
- Placement vocabulary is relations on named Parts (e.g. on top of, centered, flush to a face, offset). Unknown anchors or names are errors. Explicit cm offsets are an escape within a relation, not the default.
- v1 shapes: box, tile/array, circle/disc/ring, hollow. Out of v1: blob, carve, run_x/run_y, States, Pieces.
- A `raw` block in the Layout accepts voxel calls the DSL cannot express. It goes through the same validation, and its use is counted in the report.
- Validation: model bounds versus Spec size within 1 voxel; no floating Parts (connected-component check); no same-colour neighbouring Parts; all colour keys exist in the palette; Part names and anchors resolve.
- Single entry point (the test seam): `compile(element_dir)` returns the voxel grid and a report (errors, warnings, bounds, per-colour counts, raw-block count, ASCII slices).
- Two commands wrap it: `check <dir>` prints the report and preview; `build <dir>` generates the self-contained `build.py` with the engine inlined, replacing the manual bundle step.
- Generated `build.py` is output only; the Layout is the authored artifact. The Spec header comment and the Blender run flow stay as today.
- The 3-model proof is Monitor (boxes and tiles), Mug (round shapes) and Server1U (detailed). Other Elements are untouched until the proof passes.

### Layout format (decided in grilling)

- **Axes and origin:** same as the existing engine: +z up, -y front, x centred at 0, and the first Part sits on z=0. This keeps generated output comparable with the current builds in the 3-model proof.
- **Faces:** top, bottom, front (-y), back (+y), left (-x), right (+x).
- **Relations (small fixed set):** `on=<part>.<face>` (attach to that face of another Part), `align=center-x|center-y|left|right`, `y=flush-front|flush-back`, `gap=<cm>`, `offset_x|offset_y|offset_z=<cm>`. Anything outside this set must go in a `raw` block. Unknown names are errors.
- **Sizes:** read from the Spec. A Spec size with fewer than three dimensions (e.g. "1 cm", "2 x 1 cm") fills the missing axes with 1 voxel, so the Part is a flush decal on its host face. A Layout `size=` can override this.
- **Repeats:** `repeat={x=<count>, pitch_x=<cm>}`, with the same form for y and z. Pitch is in cm and converted per the Element's Grid (S or F).
- **Illustrative shape of a Layout (Monitor; exact values settle during the proof):**

```toml
[base]   # z=0 by default as the first Part
align = "center-x"
[neck]
on = "base.top"
y = "flush-back"
[bezel]
on = "neck.top"
offset_z = -4          # overlaps the neck
[buttons]
on = "bezel.front"
repeat = { x = 4, pitch_x = 4 }
```

## Testing Decisions

- Good tests assert external behaviour only: given an Element folder, the compiled voxel grid and report are correct. They do not test parser or resolver internals.
- One seam: `compile(element_dir)`. Tests build small fixture Elements (a tiny Spec + Layout) and assert on grid cells, bounds, colour counts, and the presence/absence of specific errors.
- Failure cases are tested at the same seam: unknown anchor, unknown palette key, floating Part, same-colour neighbours, size outside tolerance, Spec/Layout name mismatch.
- The 3-model proof is a test at the same seam: compile the migrated Layout and compare bounds and per-colour counts to the existing build within 1 voxel.
- Prior art: none. The repo has no tests yet, so this introduces the first test setup (pytest under uv in the SDK package).

## Out of Scope

- States and Pieces models, blob/carve, run_x/run_y shapes.
- Migrating Elements beyond the 3-model proof.
- Any change to the Spec format, its 30-line cap, or `build_index.py`.
- Blender-side rendering, previews as images, or GLB export changes.

## v2: States

Triage: `ready-for-agent`. Supersedes "States" in Out of Scope above; Pieces stay out.

### Problem

16 of the 30 basement Elements have `states:` (Server1U, Laptop, TowerServer, PDU, RouterBox, RackFrame, ...), so v1 cannot generate their `build.py`. Server1U already has a Layout, but it only builds the `On` state. A survey of the hand-written builds (git 5ab9393) shows three kinds of state difference:

| Kind | Example | Elements |
|---|---|---|
| Colour swap on a Part | status LED `led_green` → `led_orange`; top vents `seam` → `led_orange` when Hot | 12 (Server1U, Laptop, TowerServer, RouterBox, RackFrame, HardDrive, StripLight, BatteryModule, ...) |
| Part moves or resizes | PDU breaker lever up (On) / down (Tripped); Server1U drive bay 2 ejected by 2 voxels when Failed | 3 (PDU, ACUnit, Server1U) |
| Part present in some states only | CableBundle Tidy vs Messy runs; SpareDriveBox drives only when Full; ACUnit Broken extras | 3 (CableBundle, SpareDriveBox, ACUnit) |

Per-layer procedural differences (FlameSmall/FlameLarge lean) are not covered; they need `[[raw]]` or the future `tube`/profile shape.

### User stories

1. As an agent, I want to describe a State as a few overrides on the base Layout, so that I do not repeat the whole Element per State.
2. As an agent, I want to swap a Part's colour key per State in one line, so that LED states cost one line each.
3. As an agent, I want to move, resize or hide a Part per State, so that levers, ejected bays and broken parts are expressible.
4. As an agent, I want to address one copy of a repeated Part (drive bay 2 of 3), so that a single ejected bay does not need its own Part.
5. As an agent, I want a Part that exists only in some States, so that Tidy/Messy or Full/Empty variants are expressible.
6. As an agent, I want every State validated with the same checks as the base, so that an ejected bay that floats free fails before Blender.
7. As an agent, I want `check` to show a short per-State diff (changed voxels, colour count changes) and a full preview on demand, so that my context stays small.
8. As a maintainer, I want State names in the Layout checked against the Spec's `states:` keys, so that the two cannot drift.
9. As a designer, I want the generated `build.py` to keep using `build_states`, so that objects stay named `Name_State` and the Blender flow is unchanged.

### Implementation decisions

- **Spec unchanged.** The Spec's `states:` keys give the State names and order; the prose next to each key stays human intent. All State geometry and colour lives in the Layout.
- **Base State** is the first key in the Spec's `states:`. The plain Part tables in the Layout describe it; nothing else is needed for the base.
- **Overrides** live under `[states.<Name>]`, one inline table per Part: `"<part>" = { ... }`. Allowed keys: `key` (colour), `size`, `offset_x|y|z`, `gap`, `omit = true`. An override is applied on top of the base Part, then the State is resolved from scratch (Parts anchored `on` an overridden Part move with it).
- **Repeat instances** are addressed as `"<part>#<n>"` (1-based, x then y then z order). An instance override detaches that copy from the group; the rest keep their alignment. A `size` override grows the copy away from its host face, so it stays attached.
- **State-only Parts:** a base Part table may carry `states = ["Messy"]`; it is placed only in those States. It must still be in the Spec Parts list.
- **Raw per State:** `[[states.<Name>.raw]]` with `code = "..."`, run after the base raw blocks. Counted in the report like other raw blocks.
- **Errors:** unknown State name, unknown Part or instance in an override, unknown override key, `#n` out of range, `states = [...]` naming an unknown State. Every State runs the full v1 checks (bounds vs Spec within 1 voxel, floating Parts, palette keys, same-colour neighbours); messages are prefixed `[<State>]`. The Spec size describes the base State, so in other States a bounds mismatch is a warning (an ejected bay may stick out).
- **Seam:** `compile(element_dir, state=None)` returns the grid and report for that State (`None` = base). `check` compiles every State; it prints the base preview in full and one line per other State: `Hot: 16 voxels changed (seam -8, led_orange +8)`. `check <dir> --state Hot` prints that State's full preview.
- **Origin:** every State uses the base State's centring shift, so States line up when swapped in place.
- **Generation:** `build` emits one `def model(state)` and `R = build_states('<Name>', model, [<states>], ...)`. Ops identical in every State are emitted once; ops that differ get an inline `{'On': ..., 'Hot': ...}[state]` for a colour-only change, or an `if state in (...):` block for a geometry change or omission.

### Layout format (Server1U, illustrative)

```toml
# base Parts as today = State "On" (first key in the Spec)
["top vents"]
on = "chassis.top"
...

[states.Hot]
"status LED" = { key = "led_orange" }
"top vents"  = { key = "led_orange" }

[states.Off]
"status LED"   = { key = "led_off" }
"activity LED" = { key = "led_off" }

[states.Failed]
"status LED"    = { key = "led_red" }
"activity LED"  = { key = "led_off" }
"drive bays#2"  = { size = [4.7, 2.3, 3] }   # 3 voxels deep: sticks out 2 voxels
```

### Testing decisions

- Same seam: `compile(element_dir, state=...)`. Fixture Elements with 2-3 States assert on grid cells and colour counts per State.
- Failure cases at the seam: unknown State, unknown override Part, `#n` out of range, an override that makes a Part float, a State-only Part missing from the Spec.
- Proof: Server1U (colour swaps plus an instance move), Laptop (colour swaps only) and PDU (Part moves between States). Each State is compared to the hand-written build in git 5ab9393 within 1 voxel, measured the same way as the v1 REFERENCE table. Exception: the old Server1U build never ejected drive bay 2 although its Spec asks for it, so `Failed` is compared on bounds and colour counts with that bay's voxels excluded. After the proof passes, Server1U's `build.py` is generated and joins `test_checked_in_build_py_is_current`.

### Out of scope (v2)

- Pieces (Room_Basement and other rooms).
- Per-layer procedural shapes (Flame lean, pipe profiles).
- Animations or transitions between States; States stay separate swapped models.

## Further Notes

- About 95% of the repo's lines are bundled engine copies, not agent-authored code. The generator removes most of that by making `build.py` generated output.
- Primitive usage across current builds: box (258) and tile (87) dominate; circle/ring/hollow total about 20. Use this to prioritise DSL ergonomics.
- Success is measured as: voxel-identical or within 1 voxel on the 3-model proof. Authored line count and files read per Element are tracked but are not gates.
