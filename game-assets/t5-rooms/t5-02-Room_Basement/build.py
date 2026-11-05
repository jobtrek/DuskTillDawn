# Room_Basement (t5-02) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: Room_Basement
#   id: t5-02
#   tier: T5
#   room: Basement
#   priority: V1
#   size_cm: 800x600x240
#   voxels: 512x384x154
#   grid: S
#   grain: 0
#   collider: true
#   pieces: Floor, Wall_Back, Wall_Front, Wall_Left, Wall_Right, Ceiling
#   wall_cm: 12
#   ---
#
#   # Room_Basement
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel Room Basement for the Basement room, about 800x600x240 cm (512x384x154 voxels). Main forms: floor drain, concrete seams, pipe brackets, conduit, wall stains.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: floor drain, concrete seams, pipe brackets, conduit, wall stains.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2/3] floor drain 25 x 23 cm grate in a metal frame at the room centre-front, metal / metal_dark / seam slots
#   - [2/3] concrete seams 1 voxel grooves: floor slabs every 2 m, wall pour joints every 2 m + one form line at 1.3 m, seam
#   - [2/3] pipe brackets 6 x 6 x 25 cm on Wall_Back (5), Wall_Left (3), Wall_Right (3), metal_dark + metal arms
#   - [2/3] conduit 3 cm metal runs with clips + junction boxes on Wall_Back / Wall_Left / Wall_Right / Ceiling
#   - [2/3] wall stains damp patches 40-80 cm, stain / damp / rust streaks
#   - [2/3] stair door hole in `Room_Basement_Wall_Right` 100 x 205 cm (64 x 131 voxels), centre y = +2.1 m
#   - [2/3] painted lines 6 cm warning_yellow aisle line, drain ring, door approach lines, door-edge stripes

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t5-rooms/t5-02-Room_Basement'
exec(open(os.path.normpath(os.path.join(_here, '../../sdk/src/voxel_sdk/engine_blender.py'))).read())

# ======================================================================
# 2. COLOUR PALETTE (linear RGB 0-1)
# ======================================================================
PALETTE = {
    'chassis': (0.18, 0.19, 0.21), 'chassis_dark': (0.08, 0.09, 0.10),
    'trim': (0.45, 0.46, 0.48), 'seam': (0.02, 0.02, 0.03),
    'metal': (0.55, 0.56, 0.58), 'metal_dark': (0.22, 0.23, 0.25),
    'rust': (0.35, 0.14, 0.05), 'copper': (0.65, 0.30, 0.12),
    'glass': (0.35, 0.55, 0.65), 'screen': (0.02, 0.05, 0.08),
    'rubber': (0.04, 0.04, 0.04),
    'plastic_white': (0.80, 0.80, 0.78), 'plastic_black': (0.06, 0.06, 0.07),
    'label': (0.85, 0.82, 0.70), 'label_text': (0.10, 0.10, 0.12),
    'led_green': (0.05, 0.90, 0.15), 'led_yellow': (0.95, 0.75, 0.05), 'led_orange': (0.98, 0.45, 0.03),
    'led_red': (0.95, 0.08, 0.05), 'led_off': (0.10, 0.12, 0.10),
    'wood': (0.40, 0.24, 0.11), 'wood_dark': (0.22, 0.12, 0.05),
    'fabric': (0.20, 0.30, 0.50), 'fabric_dark': (0.10, 0.15, 0.28),
    'paint_wall': (0.62, 0.64, 0.60), 'paint_floor': (0.30, 0.32, 0.33),
    'concrete': (0.40, 0.40, 0.39),
    'warning_yellow': (0.95, 0.70, 0.05), 'warning_red': (0.80, 0.05, 0.04),
}


# ======================================================================
# 3. MODEL: Room_Basement
# ======================================================================
C = {**PALETTE, 'stain': (0.26, 0.28, 0.25), 'damp': (0.19, 0.23, 0.21)}
# DESCRIPTION: Concrete basement shell, 800x600 cm interior, 8-voxel (12 cm) slabs and walls, built as 6 separate pieces (floor, 4 walls, ceiling) in one shared frame. Stair door hole in the right wall.
# UNITS: VOXEL_SIZE (1.56 cm); interior x -256..255, y -192..191, z 8..161 (154 high); floor slab z 0..7, ceiling slab z 162..169; -y front, +x stair side.
S = VOXEL_SIZE
def piece(name, V):
    # build the piece, then move the new object back to its shared-frame position (build centres every object)
    lo, hi = V.bounds()
    R = build(name, V, C, size=S, grain=0, collider=True, metallic=('metal',))
    bpy.data.objects[name].location = ((lo[0] + hi[0] + 1) / 2 * S, (lo[1] + hi[1] + 1) / 2 * S, lo[2] * S)
    return {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
out = {}

# ---------------- floor
V = Grid()
# floor slab
box(V, -264, 263, -200, 199, 0, 7, 'paint_floor')
# slab joints: three lines across, three along
for x in (-128, 0, 128):
    box(V, x, x, -200, 199, 7, 7, 'seam')
for y in (-96, 0, 96):
    box(V, -264, 263, y, y, 7, 7, 'seam')
# yellow aisle line along the front
box(V, -240, 240, -160, -157, 7, 7, 'warning_yellow')
# door approach lines running to the stair door
box(V, 176, 179, 60, 190, 7, 7, 'warning_yellow')
box(V, 224, 227, 60, 190, 7, 7, 'warning_yellow')
# yellow ring round the floor drain
box(V, -15, 14, -75, -74, 7, 7, 'warning_yellow')
box(V, -15, 14, -47, -46, 7, 7, 'warning_yellow')
box(V, -15, -14, -75, -46, 7, 7, 'warning_yellow')
box(V, 13, 14, -75, -46, 7, 7, 'warning_yellow')
# floor drain: metal frame, dark grate, slots
box(V, -9, 8, -69, -52, 7, 7, 'metal')
box(V, -7, 6, -67, -54, 7, 7, 'metal_dark')
tile(V, -6, -6, -66, -55, 7, 7, 'seam', nx=6, dx=2)
# rust stain next to the drain
box(V, -13, -11, -62, -58, 7, 7, 'rust')
out['Floor'] = piece('Room_Basement_Floor', V)

# ---------------- back wall (inner face is the y = 192 layer)
V = Grid()
# wall slab
box(V, -264, 263, 192, 199, 8, 161, 'concrete')
# pour joints and one form line
for x in (-128, 0, 128):
    box(V, x, x, 192, 192, 8, 161, 'seam')
box(V, -256, 255, 192, 192, 84, 84, 'seam')
# damp stains: bottom-left patch with a streak above it, top-right patch
box(V, -170, -140, 192, 192, 8, 48, 'stain')
box(V, -160, -150, 192, 192, 49, 70, 'stain')
box(V, -157, -153, 192, 192, 71, 92, 'damp')
box(V, 60, 110, 192, 192, 120, 161, 'stain')
box(V, 70, 100, 192, 192, 100, 119, 'stain')
# conduit run near the ceiling with clips
box(V, -250, 250, 190, 191, 130, 131, 'metal')
tile(V, -244, -243, 189, 191, 128, 133, 'metal_dark', nx=7, dx=80)
# vertical conduit drop to a junction box
box(V, 200, 201, 190, 191, 60, 130, 'metal')
box(V, 196, 205, 188, 191, 52, 59, 'chassis')
box(V, 196, 205, 188, 188, 55, 55, 'seam')
# five pipe brackets: back plate plus two arms
for x in range(-200, 201, 100):
    box(V, x - 3, x + 2, 190, 191, 58, 73, 'metal_dark')
    box(V, x - 3, x + 2, 188, 189, 58, 61, 'metal')
    box(V, x - 3, x + 2, 188, 189, 70, 73, 'metal')
# rust streak under the middle bracket
box(V, -2, 0, 192, 192, 20, 57, 'rust')
out['Wall_Back'] = piece('Room_Basement_Wall_Back', V)

# ---------------- front wall (inner face is the y = -193 layer)
V = Grid()
# wall slab
box(V, -264, 263, -200, -193, 8, 161, 'concrete')
# pour joints and one form line
for x in (-128, 0, 128):
    box(V, x, x, -193, -193, 8, 161, 'seam')
box(V, -256, 255, -193, -193, 84, 84, 'seam')
# one damp patch
box(V, 100, 140, -193, -193, 8, 40, 'stain')
box(V, 110, 125, -193, -193, 41, 66, 'damp')
# conduit with clips
box(V, -250, 250, -192, -191, 130, 131, 'metal')
tile(V, -244, -243, -192, -190, 128, 133, 'metal_dark', nx=7, dx=80)
out['Wall_Front'] = piece('Room_Basement_Wall_Front', V)

# ---------------- left wall (inner face is the x = -257 layer)
V = Grid()
# wall slab between the back and front walls
box(V, -264, -257, -192, 191, 8, 161, 'concrete')
# pour joints and one form line
for y in (-96, 0, 96):
    box(V, -257, -257, y, y, 8, 161, 'seam')
box(V, -257, -257, -192, 191, 84, 84, 'seam')
# damp stains
box(V, -257, -257, 20, 70, 8, 36, 'stain')
box(V, -257, -257, 30, 55, 37, 62, 'damp')
box(V, -257, -257, -150, -110, 110, 161, 'stain')
# conduit run with clips
box(V, -256, -255, -180, 180, 130, 131, 'metal')
tile(V, -256, -254, -170, -169, 128, 133, 'metal_dark', ny=5, dy=85)
# three pipe brackets
for y in (-100, 0, 100):
    box(V, -256, -255, y - 3, y + 2, 58, 73, 'metal_dark')
    box(V, -254, -253, y - 3, y + 2, 58, 61, 'metal')
    box(V, -254, -253, y - 3, y + 2, 70, 73, 'metal')
out['Wall_Left'] = piece('Room_Basement_Wall_Left', V)

# ---------------- right wall (inner face is the x = 256 layer), with stair door hole
V = Grid()
# wall slab between the back and front walls
box(V, 256, 263, -192, 191, 8, 161, 'concrete')
# pour joints and one form line
for y in (-96, 0, 96):
    box(V, 256, 256, y, y, 8, 161, 'seam')
box(V, 256, 256, -192, 191, 84, 84, 'seam')
# damp stain
box(V, 256, 256, -140, -100, 8, 44, 'stain')
box(V, 256, 256, -130, -115, 45, 70, 'damp')
# conduit run, vertical drop and junction box
box(V, 254, 255, -180, 90, 130, 131, 'metal')
box(V, 254, 255, -150, -149, 60, 130, 'metal')
box(V, 251, 255, -155, -144, 52, 59, 'chassis')
box(V, 251, 251, -155, -144, 55, 55, 'seam')
# three pipe brackets
for y in (-180, -120, -60):
    box(V, 254, 255, y - 3, y + 2, 58, 73, 'metal_dark')
    box(V, 252, 253, y - 3, y + 2, 58, 61, 'metal')
    box(V, 252, 253, y - 3, y + 2, 70, 73, 'metal')
# yellow hazard stripes either side of the door hole
box(V, 256, 256, 92, 95, 8, 138, 'warning_yellow')
box(V, 256, 256, 168, 171, 8, 138, 'warning_yellow')
# stair door hole: 64 wide x 131 high, from the floor up
hole = Grid()
box(hole, 256, 263, 100, 163, 8, 138, 'concrete')
carve(V, hole)
out['Wall_Right'] = piece('Room_Basement_Wall_Right', V)

# ---------------- ceiling
V = Grid()
# ceiling slab
box(V, -264, 263, -200, 199, 162, 169, 'concrete')
# slab joints on the underside
for x in (-128, 0, 128):
    box(V, x, x, -200, 199, 162, 162, 'seam')
box(V, -264, 263, 0, 0, 162, 162, 'seam')
# two conduit runs under the ceiling with clips
for y in (-100, 100):
    box(V, -250, 250, y, y + 1, 160, 161, 'metal')
    tile(V, -240, -239, y - 1, y + 2, 159, 161, 'metal_dark', nx=7, dx=80)
# damp patch on the ceiling
box(V, 120, 180, 60, 120, 162, 162, 'stain')
box(V, 135, 165, 75, 105, 162, 162, 'damp')
out['Ceiling'] = piece('Room_Basement_Ceiling', V)
result = out
