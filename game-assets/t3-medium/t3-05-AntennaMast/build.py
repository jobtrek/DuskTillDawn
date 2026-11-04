# AntennaMast (t3-05) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: AntennaMast
#   id: t3-05
#   tier: T3
#   room: Roof
#   priority: V1
#   size_cm: 30x30x100
#   voxels: 38x38x128
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # AntennaMast
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel AntennaMast for the Roof room, about 30x30x100 cm (38x38x128 voxels). Main forms: pole, base plate.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: bolts, clamp brackets, cable clips, rust.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] pole
#   - [2] base plate
#   - [3] bolts
#   - [3] clamp brackets
#   - [3] cable clips
#   - [3] rust

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t3-medium/t3-05-AntennaMast'
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
# 3. MODEL: AntennaMast
# ======================================================================
C = {**PALETTE}

# DESCRIPTION: Antenna mast: 30x30x100 cm steel pole on a bolted base plate with gussets, clamps, cable and rust.
# UNITS: 1 voxel = 1/128 m (F); x right, y back, z up; 38x38x128 voxels
S, F = VOXEL_SIZE, FINE_SIZE

def mast():
    V = Grid()
    # base plate
    box(V, -19, 18, -19, 18, 0, 2, 'metal_dark')
    # plate bolts
    tile(V, -17, -16, -17, -16, 3, 3, 'metal', nx=2, ny=2, dx=33, dy=33)
    # lower pole
    box(V, -3, 2, -3, 2, 3, 60, 'metal')
    # upper pole
    box(V, -2, 1, -2, 1, 61, 127, 'trim')
    # gussets
    for i in range(10):
        w = 10 - i; z = 3 + 2 * i
        box(V, 3, 2 + w, -1, 0, z, z + 1, 'metal_dark'); box(V, -3 - w, -4, -1, 0, z, z + 1, 'metal_dark')
        box(V, -1, 0, 3, 2 + w, z, z + 1, 'metal_dark'); box(V, -1, 0, -3 - w, -4, z, z + 1, 'metal_dark')
    # clamp brackets
    for z in (40, 85):
        box(V, -4, 3, -4, 3, z, z + 3, 'metal_dark'); box(V, -1, 0, -5, -5, z + 1, z + 2, 'metal')
    # cross arm
    box(V, -15, 14, -1, 0, 95, 96, 'trim')
    # cable
    box(V, 1, 1, 3, 3, 8, 125, 'plastic_black')
    # cable clips
    for z in (20, 55, 100): box(V, 0, 2, 3, 4, z, z + 1, 'metal')
    box(V, -3, -3, -2, 0, 10, 18, 'rust'); box(V, 2, 2, -3, -1, 25, 31, 'rust'); box(V, 1, 2, -3, -3, 45, 52, 'rust')
    # beacon tip
    box(V, -1, 0, -1, 0, 126, 127, 'led_red')
    return V
result = build('AntennaMast', mast(), C, size=F, grain=0)
