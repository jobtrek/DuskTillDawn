# SpareDriveBox (t2-07) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: SpareDriveBox
#   id: t2-07
#   tier: T2
#   room: Basement
#   priority: V1
#   size_cm: 20x30x10
#   voxels: 26x38x13
#   grid: F
#   grain: 0
#   collider: false
#   states:
#     Full: lid open, 4 drives sitting in the foam slots
#     Empty: lid open, foam slots empty
#   ---
#
#   # SpareDriveBox
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel SpareDriveBox for the Basement room, about 20x30x10 cm (26x38x13 voxels). Main forms: box, lid.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: hinge, latch, foam, labels, drives inside.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] box 20 x 30 x 5.5 cm, chassis
#   - [2] lid 20 x 30 x 1.6 cm, ajar (open ~4 cm at the front), trim
#   - [3] hinge 2x 1.6 x 1.6 x 1.6 cm at the back, metal
#   - [3] latch 3 x 0.8 x 2.3 cm at the front, metal
#   - [3] foam 18.8 x 28 x 5 cm, fabric_dark
#   - [3] labels 2x 4 x 3 cm on lid / drive, label
#   - [3] drives inside: 2x 15 x 10 x 3 cm, metal (Full only), recess chassis_dark (Empty)

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-07-SpareDriveBox'
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
# 3. MODEL: SpareDriveBox
# ======================================================================
# DESCRIPTION: Padded plastic case for spare hard drives, lid propped open at the front so the foam slots show. Full = 2 drives, Empty = bare slots. 20x30x10 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x = width, y = depth (-y front, hinge at +y), z = up.
def model(state):
    V = Grid()
    # case body
    box(V, -13, 12, -19, 18, 0, 7, 'chassis')
    # foam insert
    box(V, -12, 11, -18, 17, 1, 7, 'fabric_dark')
    # two drive slots, one front and one back
    for y0 in (-17, 4):
        if state == 'Full':
            # metal drive lying flat
            box(V, -10, 8, y0, y0 + 12, 4, 7, 'metal')
            # label on the drive
            box(V, -7, 3, y0 + 3, y0 + 9, 7, 7, 'label')
        else:
            # empty dark recess
            box(V, -10, 8, y0, y0 + 12, 4, 7, 'chassis_dark')
    # lid propped open: rises from the hinge (back) to the front
    for y in range(-19, 19):
        z = 7 + (18 - y) * 3 // 37
        box(V, -13, 12, y, y, z + 1, z + 2, 'trim')
    # label on the lid
    box(V, -5, 4, -12, -8, 11, 11, 'label')
    # metal hinges at the back
    tile(V, -9, -8, 17, 18, 7, 8, 'metal', nx=2, dx=16)
    # metal latch at the front
    box(V, -2, 1, -19, -19, 4, 7, 'metal')
    return V, PALETTE
R = build_states('SpareDriveBox', model, ['Full', 'Empty'], size=FINE_SIZE, grain=0, collider=False, metallic=('metal',))
result = {s: {k: r[k] for k in ('object', 'voxels', 'height_m', 'warnings')} for s, r in R.items()}
