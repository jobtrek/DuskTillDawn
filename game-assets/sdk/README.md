# voxel-sdk

Spec + Layout -> validated voxel model -> self-contained Blender `build.py`. No Blender needed to check.

```
uv run voxel-sdk check ../t2-small/t2-17-Monitor   # validate + ASCII preview (front/side/top)
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
