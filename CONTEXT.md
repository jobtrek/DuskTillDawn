# Dawn to Dusk

A single-player game about the only sysadmin on a solar-powered night shift, keeping a three-floor building alive until 06:00. This glossary covers the 3D asset pipeline.

## Language

**Asset table**:
The tables in `docs/3d-assets.md`. A generated index of every Element, rebuilt from the Specs. Never edited by hand.
_Avoid_: asset list, master list

**Element**:
One thing built as its own model (a Desk, a Server1U). Each Element has its own folder in `game-assets/`.
_Avoid_: object, asset, model

**Spec**:
The hand-owned `spec.md` of one Element (30 lines max). The single source of truth for that Element's size, grid, states and detail lists.
_Avoid_: build spec, asset file, spec doc

**Tier**:
A size class of Element (T1 tiny up to T6 building), which sets the palette size range and the folder.
_Avoid_: size class, category

**Grid**:
The voxel size an Element is built on: S (1/64 m) for large things, F (1/128 m) for small parts. Elements that assemble together share a grid.
_Avoid_: resolution, scale

**State**:
A variant of an Element built as its own separate model named `Name_State`, swapped in place in the engine.
_Avoid_: variant, skin

**Piece**:
One separately built part of a room shell (floor, a wall, ceiling) so the camera can hide walls.
_Avoid_: part, section

**Assembly**:
A group of built Elements put together by hand in Blender, with no build call of its own.
_Avoid_: prefab, composite

**Seam**:
A recessed gap or dark line between parts. May be 1 voxel wide, unlike every other feature (2 voxels minimum).
_Avoid_: groove, gap

**Footprint**:
The shared floor area of the building (800×600 cm) that all three floors use; the StairShaft sits outside it on the +x side.
_Avoid_: floor plan, layout
