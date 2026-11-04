# Generate build.py from Spec + Layout via an out-of-Blender SDK

Each Element's `build.py` is about 570 lines of bundled voxel engine plus 20 to 55 lines of hand-written `box`/`tile` calls with voxel coordinates computed by the agent. Across the ~65 Elements this is over 25k lines, and the agent loses context and makes placement errors in the coordinate math. We will add a pure-Python SDK (`game-assets/sdk/`, uv-managed) that takes the Spec (Parts, sizes, colour keys) plus a per-Element `layout.toml` (relative placement of Parts), validates it outside Blender, and generates the self-contained `build.py`.

## Considered options

- **Keep hand-written `build.py`, add helpers in cm:** smallest change, but the agent still does spatial math and nothing is validated before Blender.
- **Extend `spec.md` with placement:** one file, but breaks the 30-line cap and `build_index.py --check`.
- **SDK runs inside Blender only:** no fast validation loop; every check needs a Blender round trip.

## Consequences

- `build.py` becomes generated output and is never edited by hand; the `layout.toml` is the authored artifact.
- The generated `build.py` stays self-contained (engine inlined) so it still runs via Blender's Scripting tab.
- v1 covers box, tile, circle/disc/ring and hollow. States, Pieces and blob/carve stay hand-authored, with a `raw` block as the escape hatch.
- Migration is proven on Monitor, Mug and Server1U (voxel-identical or within 1 voxel) before any other Element is touched.
