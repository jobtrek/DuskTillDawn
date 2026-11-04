# Dawn to Dusk: 3D asset list

Every model to build by hand, **smallest to biggest**. You build each object yourself in Blender with the `blender-voxel-py` skill (give it the spec path), then assemble them. This list follows that skill's rules; where this file and the skill disagree, the skill wins.

Source: [game-v1.md](game-v1.md) (v1), plus parked areas from the other docs in the last section.

Each Element has its own folder `game-assets/<tier>/<id>-<Name>/` with a hand-owned `spec.md` (30 lines max), the saved `build.py`, and the exported `.glb`.

**The `spec.md` files are the source of truth.** The tables below are generated: never edit them, edit the spec, then run `uv run scripts/build_index.py` (`--check` fails if a spec is over 30 lines or the tables are stale). Shared colours live in `game-assets/palette.py`.

## 1. Rules from the skill (apply to every object)

| Rule | What it means here |
|---|---|
| **One mesh object + its materials** per build call | Each row below = one call. States are separate objects named `Name_State`. |
| **Style** | Hard edges only: boxes, no bevel, no smooth shading. Round things are stepped boxes. `blob` / `disc` only for organic parts (plant leaves, head). |
| **Real proportions** | Sizes below are real-world centimetres. Do not cartoon them. |
| **Three detail layers** | (1) primary forms, (2) secondary parts, (3) tertiary detail. Columns below list 2 and 3 as `secondary · tertiary`. An object missing a layer is not done. |
| **Palette** | Material keys per object scale by tier: **T1 6–10, T2 10–18, T3–T6 15–30**. A distinct colour per part (no two neighbours the same hue), plus highlight / shadow variants for glass, metal, faces where the object is big enough to show them. Detail items that do not fit in the voxels are merged or dropped. |
| **Seams** | A seam or recessed gap may be **1 voxel** wide. Every other feature (key, port, button, LED) is **2 voxels or more**. |
| **Axes** | +z up, **-y is the front**, centred on x/y, feet on z=0 (origin at the bottom centre). Wall-mounted items: back at +y. |
| **Build order** | Large forms first, small details last (later writes overwrite earlier ones). Model one half and `mirror_x` for symmetric objects. |
| **Materials** | `materials='palette'` (default, one texture per object). Use `'separate'` only if you want to tweak colours per material in Blender. |
| **Export** | `.glb` only when you ask. Save the `.blend` first. Front = -Y, origin at the feet. |

### Two grids (pick per object)

| Grid | Voxel | Voxels per metre | Use for |
|---|---|---|---|
| **S** = `VOXEL_SIZE` | 1/64 m = 1.56 cm | 64 | Characters (~110 voxels tall), doors (~130), furniture, large props, rooms |
| **F** = `FINE_SIZE` | 1/128 m = 7.8 mm | 128 | Hand-held props and anything with small parts |

- The smallest feature (key, port, button, LED) must be **2 voxels or more**: 3.1 cm on S, 1.6 cm on F. Seams and gaps are the one exception (1 voxel).
- Parts that **assemble together share a grid**: the rack frame and the servers are both F, the panel and its mount are both F.
- Voxel counts in the tables are real size × voxels per metre. Check `height_m` in the build result against the real height.

### Per-object settings

| Setting | Rule |
|---|---|
| `grain` | `0` for electronics, metal, glass, room shells (also keeps faces down). Default `0.08` for wood, fabric, skin, plants (Desk, Chair, Cot, Plant, Engineer). `0.15` for LotPlate. |
| `collider=True` | Floors, walls, parapets, StairFlight, StairLanding, StairShaft, Desk, StorageCabinet, MetalShelf, RackFrame, BatteryRackFrame, ACUnit, Cot. |
| Face budget | Keep each object under 40 000 faces (the skill warns above that). Rooms and the exterior: grain 0, and cut-away walls are separate objects. |

### Shared palette keys (start here, add per object)

Same key names across objects keep colours consistent. Pick the values once and reuse them.

`chassis`, `chassis_dark`, `trim`, `seam`, `metal`, `metal_dark`, `rust`, `glass`, `screen`, `rubber`, `plastic_white`, `plastic_black`, `label`, `label_text`, `led_green`, `led_yellow`, `led_red`, `led_off`, `wood`, `wood_dark`, `fabric`, `fabric_dark`, `paint_wall`, `paint_floor`, `concrete`, `warning_yellow`, `warning_red`, `copper`

### Done when (per object)

1. `build` returns without error and `warnings` is empty.
2. `height_m` matches the real height in the table.
3. Front, 3/4 and close-up views all read as the real object; no floating voxels.
4. All three detail layers are present.

Priority: **V1** = needed for the first playable. **UP** = needed when that upgrade is bought.

---

## T1: Tiny (up to 15 cm)

<!-- index:T1 -->
| ID | Element | Room | Size cm → voxels (W×D×H) | Grid | States / pieces (separate models) | Secondary · tertiary detail | Pri |
|---|---|---|---|---|---|---|---|
| [t1-01](../game-assets/t1-tiny/t1-01-PowerPlug/spec.md) | **PowerPlug** | Basement | 5×4×6 → 6×5×8 | F | one | pins, strain relief, cable stub · grip ridges both sides trim, seam ring at top of body seam, label on front face label | V1 |
| [t1-02](../game-assets/t1-tiny/t1-02-WallSwitch/spec.md) | **WallSwitch** | All | 9×2×9 → 12×3×12 | F | one | plate, rocker · screws, seam, indicator window | V1 |
| [t1-03](../game-assets/t1-tiny/t1-03-SprinklerHead/spec.md) | **SprinklerHead** | Basement | 6×6×10 → 8×8×13 | F | one | frame arms, deflector · glass bulb (2 voxels), thread, seam | UP |
| [t1-04](../game-assets/t1-tiny/t1-04-FireAlarmButton/spec.md) | **FireAlarmButton** | Basement | 10×4×10 → 13×5×13 | F | one | housing, glass cover · label, striker, screws, LED | V1 |
| [t1-05](../game-assets/t1-tiny/t1-05-Mouse/spec.md) | **Mouse** | Control | 6×11×4 → 8×14×5 | F | one | buttons, scroll wheel · seam, cable, sensor window, grip texture | V1 |
| [t1-06](../game-assets/t1-tiny/t1-06-Mug/spec.md) | **Mug** | Control | 11×8×9 → 14×10×12 | F | _Full, _Empty | handle, rim · coffee surface, logo label, base ring, chip wear | V1 |
| [t1-07](../game-assets/t1-tiny/t1-07-HardDrive/spec.md) | **HardDrive** | Basement | 10×15×3 → 13×19×4 | F | _Healthy, _Failed | casing, label · screws, connector pins, PCB edge, 2×2 LED (green / red) | V1 |
<!-- /index:T1 -->

## T2: Small (15–60 cm)

<!-- index:T2 -->
| ID | Element | Room | Size cm → voxels (W×D×H) | Grid | States / pieces (separate models) | Secondary · tertiary detail | Pri |
|---|---|---|---|---|---|---|---|
| [t2-01](../game-assets/t2-small/t2-01-Beacon/spec.md) | **Beacon** | Control + Basement | 12×12×16 → 15×15×20 | F | _Off, _On | dome, base · lens ribs, bolts, wire, bulb | V1 |
| [t2-02](../game-assets/t2-small/t2-02-ExtinguisherBracket/spec.md) | **ExtinguisherBracket** | Basement | 20×10×20 → 26×13×26 | F | one | wall plate, strap · screws, buckle, sign | V1 |
| [t2-03](../game-assets/t2-small/t2-03-Notepad/spec.md) | **Notepad** | Control | 15×21×2 → 19×27×3 | F | one | cover, spiral · ruled lines, pen + clip, sticky tabs | V1 |
| [t2-04](../game-assets/t2-small/t2-04-WallClock/spec.md) | **WallClock** | Control | 30×5×30 → 38×6×38 | F | one | frame, glass · hour ticks, hands, battery bump | V1 |
| [t2-05](../game-assets/t2-small/t2-05-WallVent/spec.md) | **WallVent** | Basement | 30×5×30 → 38×6×38 | F | one | frame, louvres · screws, dust, slats (tile) | V1 |
| [t2-06](../game-assets/t2-small/t2-06-CableBundle/spec.md) | **CableBundle** | Basement | 30×20×8 → 38×26×10 | F | _Tidy, _Messy | 4 cable colours, connectors · ties, labels, kinks | V1 |
| [t2-07](../game-assets/t2-small/t2-07-SpareDriveBox/spec.md) | **SpareDriveBox** | Basement | 20×30×10 → 26×38×13 | F | _Full, _Empty | box, lid · hinge, latch, foam, labels, drives inside | V1 |
| [t2-08](../game-assets/t2-small/t2-08-WasteBin/spec.md) | **WasteBin** | Control | 25×25×30 → 32×32×38 | F | one | rim, liner · ridges, crumpled paper, label | V1 |
| [t2-09](../game-assets/t2-small/t2-09-PipeCorner/spec.md) | **PipeCorner** | Basement | 40×40×8 → 51×51×10 | F | one | pipe, flanges · bolts, clamps, weld seam | V1 |
| [t2-10](../game-assets/t2-small/t2-10-CleanerRobot/spec.md) | **CleanerRobot** | Roof | 30×40×15 → 38×51×19 | F | one | body, brush wheels · bumper, sensor, LED, handle | UP |
| [t2-11](../game-assets/t2-small/t2-11-DeskLamp/spec.md) | **DeskLamp** | Control | 20×20×42 → 26×26×54 | F | _On, _Off | arm, shade, base · switch, hinge joints, cable, bulb | V1 |
| [t2-12](../game-assets/t2-small/t2-12-Keyboard/spec.md) | **Keyboard** | Control | 44×14×3 → 56×18×4 | F | one | case, tilt feet · individual keys, space bar, arrow cluster, cable, label | V1 |
| [t2-13](../game-assets/t2-small/t2-13-RouterBox/spec.md) | **RouterBox** | Basement | 44×25×4.4 → 56×32×6 | F | one | case, ear brackets · ports (tile), LEDs, vents, label | UP |
| [t2-14](../game-assets/t2-small/t2-14-BatteryModule/spec.md) | **BatteryModule** | Basement | 44×45×13 → 56×58×17 | F | _Charged, _Low, _Overheating | case, handles · terminals, BMS display, LED bar, vents, label | V1 |
| [t2-15](../game-assets/t2-small/t2-15-ComputerTower/spec.md) | **ComputerTower** | Control | 20×45×45 → 26×58×58 | F | one | case, front panel · drive bays, mesh vent (tile), power button, USB, LEDs, rear PSU | V1 |
| [t2-16](../game-assets/t2-small/t2-16-FlameSmall/spec.md) | **FlameSmall** | Basement | 30×30×50 → 19×19×32 | S | _A, _B | 3 flame layers (red / orange / yellow) · hot core, stepped tips, ember voxels at the base | V1 |
| [t2-17](../game-assets/t2-small/t2-17-Monitor/spec.md) | **Monitor** | Control | 54×18×36 → 69×23×46 | F | one | bezel, neck, base · power LED, buttons, back vents, VESA plate, ports | V1 |
| [t2-18](../game-assets/t2-small/t2-18-Extinguisher/spec.md) | **Extinguisher** | Basement | 16×16×55 → 20×20×70 | F | one | body, hose, handle · pin, gauge, label, nozzle, strap | V1 |
| [t2-19](../game-assets/t2-small/t2-19-Server1U/spec.md) | **Server1U** | Basement | 45×60×4.4 → 58×77×6 | F | _On, _Off, _Failed | bezel, handles · vents (tile), drive bays, 2×2 LEDs, power button, screws, label | V1 |
| [t2-20](../game-assets/t2-small/t2-20-InverterBox/spec.md) | **InverterBox** | Roof | 40×20×60 → 51×26×77 | F | _On, _Off, _Broken | housing, display · LEDs, heat fins, DC switch, cable glands, label | V1 |
| [t2-21](../game-assets/t2-small/t2-21-Plant/spec.md) | **Plant** | Control | 30×30×60 → 38×38×77 | F | one | pot, stem · leaves (blob ok, organic), soil, rim, saucer | V1 |
| [t2-22](../game-assets/t2-small/t2-22-RaidTower/spec.md) | **RaidTower** | Basement | 25×50×60 → 32×64×77 | F | one | case, bays · 8 drive caddies (tile), handles, LEDs, vents, label | UP |
| [t2-23](../game-assets/t2-small/t2-23-AntennaDish/spec.md) | **AntennaDish** | Roof | 60×40×60 → 77×51×77 | F | one | dish, feed horn, struts · ribs, mount clamp, cable, label, bolts | V1 |
<!-- /index:T2 -->

## T3: Medium (60 cm–1.3 m)

<!-- index:T3 -->
| ID | Element | Room | Size cm → voxels (W×D×H) | Grid | States / pieces (separate models) | Secondary · tertiary detail | Pri |
|---|---|---|---|---|---|---|---|
| [t3-01](../game-assets/t3-medium/t3-01-RoofHatchLid/spec.md) | **RoofHatchLid** | Roof | 90×90×6 → 58×58×4 | S | one | lid, hinge · handle, latch, seam, bolts | V1 |
| [t3-02](../game-assets/t3-medium/t3-02-RoofHatchBase/spec.md) | **RoofHatchBase** | Roof | 90×90×30 → 58×58×19 | S | one | curb, frame · gasket, bolts, corner flashing | V1 |
| [t3-03](../game-assets/t3-medium/t3-03-PipeStraight/spec.md) | **PipeStraight** | Basement | 8×100×8 → 10×128×10 | F | one | pipe, flanges · bolts, clamps, flow arrow, rust | V1 |
| [t3-04](../game-assets/t3-medium/t3-04-WallShelf/spec.md) | **WallShelf** | Control | 100×30×25 → 64×19×16 | S | one | board, brackets · screws, edge lip, wood grain | V1 |
| [t3-05](../game-assets/t3-medium/t3-05-AntennaMast/spec.md) | **AntennaMast** | Roof | 30×30×100 → 38×38×128 | F | one | pole, base plate · bolts, clamp brackets, cable clips, rust | V1 |
| [t3-06](../game-assets/t3-medium/t3-06-Chair/spec.md) | **Chair** | Control | 60×60×100 → 38×38×64 | S | one | seat, back, armrests · 5 wheels, gas lift, star base, stitching seam, lever | V1 |
| [t3-07](../game-assets/t3-medium/t3-07-BatteryRackFrame/spec.md) | **BatteryRackFrame** | Basement | 60×80×100 → 77×102×128 | F | one | posts, shelves, side panels · rail holes (tile), cable entries, casters, label | V1 |
| [t3-08](../game-assets/t3-medium/t3-08-StairLanding/spec.md) | **StairLanding** | Between floors | 100×100×100 → 64×64×64 | S | one | platform, railing · posts, handrail (stepped), anti-slip strip, bolts | V1 |
| [t3-09](../game-assets/t3-medium/t3-09-PanelMount/spec.md) | **PanelMount** | Roof | 110×60×70 → 141×77×90 | F | one | legs, rails · bolts, braces, clamps, rust, feet | V1 |
| [t3-10](../game-assets/t3-medium/t3-10-StripLight/spec.md) | **StripLight** | Control + Basement | 120×10×6 → 154×13×8 | F | _On, _Off | housing, diffuser · end caps, brackets, chain hangers, wires | V1 |
| [t3-11](../game-assets/t3-medium/t3-11-CableTray/spec.md) | **CableTray** | Basement | 30×120×10 → 38×154×13 | F | one | side rails, rungs · brackets, bolts, cables, ties | V1 |
| [t3-12](../game-assets/t3-medium/t3-12-Window/spec.md) | **Window** | Control | 120×12×120 → 77×8×77 | S | one | frame, glass · glazing bars, handle, sill, hinge | V1 |
| [t3-13](../game-assets/t3-medium/t3-13-RackFrame/spec.md) | **RackFrame** | Basement | 60×80×120 → 77×102×154 | F | one | posts, side panels, door · rail holes (tile), vent slots, casters, lock, label (24U) | V1 |
| [t3-14](../game-assets/t3-medium/t3-14-FlameLarge/spec.md) | **FlameLarge** | Basement | 80×80×120 → 51×51×77 | S | _A, _B | 3 flame layers (red / orange / yellow) · hot core, stepped tips, ember voxels at the base | V1 |
| [t3-15](../game-assets/t3-medium/t3-15-Broom/spec.md) | **Broom** | Roof | 40×6×130 → 51×8×166 | F | one | handle, head · bristle rows (tile), bolts, grip tape, wear | V1 |
<!-- /index:T3 -->

## T4: Large (over 1.3 m)

<!-- index:T4 -->
| ID | Element | Room | Size cm → voxels (W×D×H) | Grid | States / pieces (separate models) | Secondary · tertiary detail | Pri |
|---|---|---|---|---|---|---|---|
| [t4-01](../game-assets/t4-large/t4-01-Desk/spec.md) | **Desk** | Control | 160×80×75 → 102×51×48 | S | one | top, side panels, drawer · handles, cable grommet, edge trim, screws, wood grain | V1 |
| [t4-02](../game-assets/t4-large/t4-02-SolarPanel/spec.md) | **SolarPanel** | Roof | 100×4×165 → 128×5×211 | F | _Clean, _Dirty, _Damaged | frame, glass · 60 cells (tile), busbars, junction box, corner keys, label | V1 |
| [t4-03](../game-assets/t4-large/t4-03-SuppressionTank/spec.md) | **SuppressionTank** | Basement | 30×30×170 → 19×19×109 | S | one | cylinder (stepped), valve · gauge, hose, band, label, bracket | UP |
| [t4-04](../game-assets/t4-large/t4-04-Engineer/spec.md) | **Engineer** | All | 50×30×172 → 32×19×110 | S | _Head, _Torso, _Arm, _Leg | head (blob ok, organic), hoodie · face, hair, pockets, lanyard badge, belt, shoes, hands | V1 |
| [t4-05](../game-assets/t4-large/t4-05-MetalShelf/spec.md) | **MetalShelf** | Basement | 100×40×180 → 64×26×115 | S | one | 5 shelves, posts · bolts, labelled bins, boxes, rust | V1 |
| [t4-06](../game-assets/t4-large/t4-06-StorageCabinet/spec.md) | **StorageCabinet** | Control | 90×45×180 → 58×29×115 | S | one | body, doors · handles, lock, vent slots (tile), hinge seams, feet, label | V1 |
| [t4-07](../game-assets/t4-large/t4-07-Cot/spec.md) | **Cot** | Control | 190×80×50 → 122×51×32 | S | one | frame, mattress · pillow, blanket folds, stitching, legs | V1 optional |
| [t4-08](../game-assets/t4-large/t4-08-ACUnit/spec.md) | **ACUnit** | Basement | 70×90×200 → 45×58×128 | S | _On, _Off, _Broken | cabinet, doors · display, louvres, handles, logo, LED, pipe stubs, feet | V1 |
| [t4-09](../game-assets/t4-large/t4-09-Door_Office/spec.md) | **Door_Office** | Control | 100×10×205 → 64×6×131 | S | one | slab, frame · lever handle, 3 hinges, lock plate, kick plate, panel grooves, sign | V1 |
| [t4-10](../game-assets/t4-large/t4-10-Door_Steel/spec.md) | **Door_Steel** | Basement | 100×10×205 → 64×6×131 | S | one | slab, frame · push bar, 3 hinges, lock, warning sign, rivets, dents | V1 |
| [t4-11](../game-assets/t4-large/t4-11-StairFlight/spec.md) | **StairFlight** | Between floors | 100×225×140 → 64×144×90 | S | one | treads, stringers, railing · nosing strips, anti-slip, balusters, bolts, wear | V1 |
<!-- /index:T4 -->

## T5 + T6: Rooms and building (grid S, collider on, grain 0)

Rooms are built as **separate pieces** so the isometric camera can hide the near walls. Furniture is not part of the shell.

**Footprint.** All three floors share one **800 × 600 cm** interior footprint. The StairShaft (150 × 350 cm) sits outside it on the **+x side**, so each floor's `Wall_Right` has the stair door. The Basement is underground and only shown in cutaway.

**Camera.** Fixed in v1 (no rotation). `Wall_Front` and `Wall_Left` are hidden; `Wall_Back` and `Wall_Right` always stay. Wall-mounted props (WallClock, WallSwitch, WallVent, ExtinguisherBracket, WallShelf, Window, StripLight) go on `Wall_Back` or `Wall_Right`. Camera rotation is parked.

**Exterior.** `BuildingExterior` is shown in the menu and night report only. In-game you see the room pieces. Its roof edge must match the `Room_Roof` parapets.

<!-- index:T56 -->
| ID | Element | Where | Size cm → voxels (W×D×H) | Pieces | Detail | Pri |
|---|---|---|---|---|---|---|
| [t5-01](../game-assets/t5-rooms/t5-01-Room_Control/spec.md) | **Room_Control** | Control room | 800×600×280 → 512×384×179 | Floor, Wall_Back, Wall_Front, Wall_Left, Wall_Right, Ceiling | skirting, wall panel seams, outlets, conduit, floor tiles (tile), stair door hole in `Room_Control_Wall_Right` + office door + window holes (carve), paint wear | V1 |
| [t5-02](../game-assets/t5-rooms/t5-02-Room_Basement/spec.md) | **Room_Basement** | Basement | 800×600×240 → 512×384×154 | Floor, Wall_Back, Wall_Front, Wall_Left, Wall_Right, Ceiling | floor drain, concrete seams, pipe brackets, conduit, wall stains, stair door hole in `Room_Basement_Wall_Right`, painted lines | V1 |
| [t5-03](../game-assets/t5-rooms/t5-03-Room_Roof/spec.md) | **Room_Roof** | Roof | 800×600×100 → 512×384×64 | Floor, Parapet_Back, Parapet_Front, Parapet_Left, Parapet_Right, Chimney | coping, drain outlets, tar seams, hatch hole, vents, gutter | V1 |
| [t5-04](../game-assets/t5-rooms/t5-04-StairShaft/spec.md) | **StairShaft** | Between floors | 150×350×280 → 96×224×179 | one | walls, door holes (into each floor's `Wall_Right`), light fixture mounts, wall trim | V1 |
| [t6-01](../game-assets/t6-building/t6-01-BuildingExterior/spec.md) | **BuildingExterior** | Whole building | 1000×700×400 → 640×448×256 | one | windows (tile), entrance door, wall panels, downpipes, outdoor AC, roof edge (matches the `Room_Roof` parapets), signage | V1 |
| [t6-02](../game-assets/t6-building/t6-02-LotPlate/spec.md) | **LotPlate** | Whole building | 2000×1600×10 → 1280×1024×6 | one | gravel, paving slabs, paint lines, drain covers, kerb. grain | V1 |
<!-- /index:T56 -->

Wall thickness 12 cm = 8 voxels. Cut door and window holes with `carve`.

---

## 2. Assemblies (you build these in Blender from the objects above, no build call)

| Assembly | Made from | Variants |
|---|---|---|
| **MainComputerStation** | Desk + 3 × Monitor + Keyboard + Mouse + ComputerTower + Chair + DeskLamp + Mug + Notepad | normal / alarm (Beacon_On nearby) |
| **ServerRack_Full** | RackFrame + 8 × Server1U | normal (all `_On`), hot, on fire (swap in `Server1U_Failed` + FlameSmall / FlameLarge) |
| **BatteryRack_Full** | BatteryRackFrame + 6 × BatteryModule | normal, hot (`_Overheating`) |
| **SolarArray** | 6 × SolarPanel + PanelMount | clean, dirty, damaged (swap panel objects) |
| **PipeNetwork** | PipeStraight + PipeCorner, cloned | one |
| **SprinklerSystem** (UP) | PipeStraight + SprinklerHead | one |
| **SecondAC** (UP) | ACUnit | reuse |
| **Roof_Furnished / Control_Furnished / Basement_Furnished** | Room shells + all props for that floor | one each |
| **Building_Assembled** | 3 furnished floors + StairShaft | for menu and night report |

---

## 3. Not built with the skill

| Thing | How instead |
|---|---|
| Status LEDs | Painted as 2×2 voxel colour keys on the host object (too small to be objects) |
| Hardware security key (2FA upgrade) | UI icon only; 18 × 5 mm is below the 2-voxel rule |
| Screen content | Texture / UI in Godot on the `screen` key |
| Smoke, sparks, steam, water spray | Godot particles |
| Night sky | Godot environment (the skill never touches the world) |
| Light flicker, door opening, dish rotation, hatch lid | Engine animation |
| Character animation (idle, walk, carry, repair, shock) | Rig the 4 Engineer parts in Blender yourself |

---

## 4. Parked areas (model later, same rules)

| Area | Objects needed |
|---|---|
| **Generator yard** | Diesel generator, fuel tank, transfer switch cabinet, fence, tanker truck |
| **Battery shed** | Shed shell, BMS panel (reuse BatteryRackFrame) |
| **Delivery dock** | Roll-up door, delivery truck, inspection bench, scanner, quarantine cage, parts shelf, parcels |
| **Mini substation** | Switch cabinet, transformer, levers |
| **Fiber trench** | Trench, cable, splice box |
| **Front gate** | Gate, badge reader, guard booth |
| **Parking lot** | Cars, lamp posts |
| **Weather / outdoor extras** | Snow panel state, golf cart, drone, flashlight, sandbags, water pump |
| **Animals** | Squirrel, rat, bird |
| **Co-op** | Second character, walkie-talkie |
| **Cosmetics** | Outfits, hats |
| **Perimeter fence** | One object |

---

## 5. Suggested build order

1. **Ten small things first** (HardDrive, Server1U, Mug, Extinguisher, Monitor). They settle the palette and workflow.
2. **Then the parts of the three most-seen assemblies**: RackFrame + Server1U, SolarPanel + PanelMount, Desk + Monitor.
3. **Then the other props**, floor by floor.
4. **Then Engineer** (4 parts) and rig it.
5. **Then the room shells** and place everything.
6. **Last:** UP objects, BuildingExterior, LotPlate.

## 6. Counts

| Group | V1 objects | UP objects |
|---|---|---|
| Props and furniture (T1–T4) | 73 | 5 |
| Rooms and building (T5–T6) | 21 | 0 |
| **Total** | **94** | **5** |

Counts include every state and every Engineer part as its own object (one build call each).
