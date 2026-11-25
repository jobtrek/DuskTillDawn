# DuskTillDawn

A single-player game about the only sysadmin on a solar-powered night shift, keeping a three-floor building alive from 20:00 until 08:00. This glossary covers the game's core terms and the 3D asset pipeline.

## Language

**Incident**:
Something that goes wrong during a night and that the player must fix (an expired certificate, a phishing mail, a failing drive). It moves through Warning, Active, Escalated and Resolved.
_Avoid_: défi, challenge, problem, panne

**Server**:
One named machine in the server room (WEB-01, DB-01, MAIL-01…, the same names the Mails use). It hosts Clients, has a load and a status: OK, WARN or DOWN. An Incident hits one Server, a circuit of two, or every Server.
_Avoid_: machine, host, node, box

**Client**:
A customer company hosted on a Server. When its Server breaks, the SLA drains by the number of Clients hurt. Every Client counts the same in v1.
_Avoid_: customer, user, tenant
**Electrical fault**:
An Incident in the Basement power equipment. If ignored it escalates into a Fire. It is the only cause of Fire; heat alone never starts one.
_Avoid_: short circuit, electrical fire (for the Warning stage)

**Auto-shutdown**:
The servers switching themselves off when the room gets too hot. They stay off until the room cools and the player reboots them at the racks.
_Avoid_: crash, overheat fire

**Start charge**:
The battery level at 20:00, set by how clean the solar panels were during the day.
_Avoid_: initial power, day charge

**Eco Mode**:
A low-power mode the player can turn on when the battery runs low: less Traffic served, slower drain.
_Avoid_: load-shed mode, power saving

**Mail**:
A message in the computer's Mail app. Three kinds: a Warning mail (early sign of an Incident), a News mail (tech news, flavour and hints) and a Phishing mail (a fake that the player must report). The inbox empties at the start of each night.
_Avoid_: email, e-mail, message

**App**:
A program on the desk computer: Mail, Monitoring or Admin. The player opens it from the computer's desktop and closes it with Esc or its close button.
_Avoid_: window, screen, program

**Dashboard**:
The small HUD panel (top right) with points, failures and every running Incident. Not the computer's Monitoring app.
_Avoid_: monitoring (for the HUD panel)

**Asset table**:
`game-assets/INDEX.md`. A generated index of every Element, rebuilt from the Specs by `voxel-sdk index`. Never edited by hand.
_Avoid_: asset list, master list

**Element**:
One thing built as its own model (a Desk, a Server1U). Each Element has its own folder in `game-assets/`.
_Avoid_: object, asset, model

**Spec**:
The hand-owned `spec.md` of one Element (40 lines max). The single source of truth for that Element's size, grid, states and detail lists.
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
One separately built section of a room shell (floor, a wall, ceiling) so the camera can hide walls.
_Avoid_: section

**Part**:
One named shape inside an Element (the bezel, neck, base of a Monitor). Listed with a size and colour key in the Spec, and placed relative to other Parts in the Layout.
_Avoid_: component, shape

**Layout**:
The `layout.toml` of one Element. Says where each Part sits relative to the others; the Spec says what the Parts are. Generated build output is never edited by hand.
_Avoid_: model file, placement file

**Assembly**:
A group of built Elements put together by hand in Blender, with no build call of its own.
_Avoid_: prefab, composite

**Seam**:
A recessed gap or dark line between parts. May be 1 voxel wide, unlike every other feature (2 voxels minimum).
_Avoid_: groove, gap

**Footprint**:
The shared floor area of the building (800×600 cm) that all three floors use; the StairShaft sits outside it on the +x side.
_Avoid_: floor plan, layout

**Stage**:
One step of the server progression, 1 (single laptop) to 5 (multiple racks). In v1 the player buys the next Stage in the shop with points. Not to be confused with Tier, which is an Element size class.
_Avoid_: tier (for the progression), level

**Night den**:
The small corner of the Basement near the stair door where the sysadmin's personal belongings collect. It grows over the Stages and never covers an incident cue.
_Avoid_: cozy corner, break area
