# voxel-sdk

Spec + Layout -> validated voxel model -> self-contained Blender `build.py`. No Blender needed to check.

```
uv run voxel-sdk check ../t2-small/t2-17-Monitor   # validate + ASCII preview (front/side/top)
uv run voxel-sdk check ../t2-small/t2-26-PDU --state Tripped   # full preview of one State
uv run voxel-sdk build ../t2-small/t2-17-Monitor   # write build.py (generated, never hand-edited)
uv run pytest
```

An Element folder holds `spec.md` (Parts: name, cm size, colour key) and `layout.toml` (where Parts sit).
Palette comes from the nearest `palette.py` above the folder. Grid S = 1/64 m, F = 1/128 m per voxel.

## layout.toml reference

One table per Part, in placement order; the first Part sits at x/y centre, z=0. Names must match the Spec Parts list.

| key | meaning |
|---|---|
| `on = "part.face"` | attach outside that face (top, bottom, front=-y, back=+y, left=-x, right=+x), centred on the other two axes. Required except on the first Part |
| `align` | string or list: `center-x` `left` `right` `center-y` `center-z` `bottom` `top` (not on the axis of `on`) |
| `y` | `flush-front` or `flush-back` |
| `gap` | cm along the `on` direction (negative sinks the Part into the host) |
| `offset_x/y/z` | cm shift after placement |
| `size = [x, y, z]` | cm; replaces the Spec size. Missing axes are 1 voxel (flush decal) |
| `repeat = { x = 4, pitch_x = 3 }` | count and pitch in cm, also y, z. Alignment uses the whole group |
| `shape` | `box` (default) `hollow` `disc` `circle` `ring` |
| `thickness`, `open` | cm wall for hollow/ring; `open = ["top", ...]` leaves a hollow face open |
| `[[raw]]` `code = "..."` | escape hatch: engine calls on `V` (box, hollow, tile, disc, blob, carve, Grid, shell, ellipse). Same checks; counted in the report |

Checks: unknown Part/anchor/face/key, unknown palette key, size vs Spec `voxels:` within 1 voxel, floating Parts (errors);
same-colour neighbouring Parts, Spec Parts missing from the Layout (warnings).

## States

The Spec's `states:` keys name the States; the first is the base State, described by the plain Part tables.
Each other State is a table of overrides (examples: `t2-26-PDU`, `t2-24-Laptop`, `t2-19-Server1U`):

```toml
[states.Tripped]
"status LED"    = { key = "led_red" }                       # colour swap
"breaker lever" = { key = "warning_red", offset_z = -1.56 } # move
"drive bays#2"  = { size = [4.7, 2.3, 3] }                  # one copy of a repeat; grows away from its host face
```

| key | meaning |
|---|---|
| `key` `size` `offset_x/y/z` `gap` | same as the Part keys, for that State only |
| `omit = true` | Part (or copy) absent in that State |
| `states = ["Messy"]` | on a Part table: the Part exists only in those States |
| `[[states.<Name>.raw]]` | raw block run only in that State |

`check` prints the base State in full and one diff line per other State. Non-base States may exceed the Spec size (warning, not error).

