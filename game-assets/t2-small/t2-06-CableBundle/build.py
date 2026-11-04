# CableBundle (t2-06) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: CableBundle
#   id: t2-06
#   tier: T2
#   room: Basement
#   priority: V1
#   size_cm: 30x20x8
#   voxels: 38x26x10
#   grid: F
#   grain: 0
#   collider: false
#   states:
#     Tidy: cables parallel, a cable tie every ~6 cm
#     Messy: tangled loops, no ties, one kinked cable
#   ---
#
#   # CableBundle
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel CableBundle for the Basement room, about 30x20x8 cm (38x26x10 voxels). Main forms: 4 cable colours, connectors.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: ties, labels, kinks.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] 4 cables 1.6 cm thick: cable_red, cable_yellow, cable_blue, cable_green
#   - [2] connectors 4x 3 x 1.6 x 1.6 cm, plastic_black + metal tip
#   - [3] cable ties 3x, plastic_white (Tidy) / 1x (Messy)
#   - [3] labels 2x 1.6 cm, label
#   - [3] kinks: sharp bends only in Messy

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-06-CableBundle'
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
# 3. MODEL: CableBundle
# ======================================================================
C = {**PALETTE, 'cable_red': (0.80, 0.06, 0.05), 'cable_yellow': (0.90, 0.70, 0.05),
     'cable_blue': (0.05, 0.18, 0.80), 'cable_green': (0.05, 0.50, 0.12)}
# DESCRIPTION: Four coloured network/power cables with connectors at both ends. Tidy = tied into a straight bundle, Messy = crossed and kinked. 30x20x8 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x = length, y = depth (-y front), z = up.
COL = ['cable_red', 'cable_yellow', 'cable_blue', 'cable_green']
def model(state):
    V = Grid()
    if state == 'Tidy':
        # four cables in a 2x2 bundle
        for i, c in enumerate(COL):
            y, z = (-2 + 2 * (i % 2)), 1 + 2 * (i // 2)
            # straight cable run
            box(V, -15, 14, y, y + 1, z, z + 1, c)
            # connector body at both ends
            box(V, -19, -16, y, y + 1, z, z + 1, 'plastic_black')
            box(V, 15, 18, y, y + 1, z, z + 1, 'plastic_black')
            # metal connector tips
            box(V, -19, -19, y, y + 1, z, z + 1, 'metal')
            box(V, 18, 18, y, y + 1, z, z + 1, 'metal')
        # white cable ties
        tile(V, -9, -9, -3, 2, 0, 4, 'plastic_white', nx=3, dx=9)
        # paper labels on two cables
        box(V, -5, -4, -3, -3, 1, 2, 'label')
        box(V, 5, 6, -3, -3, 1, 2, 'label')
    else:
        # four cables crossing at different heights, each with sharp kinks
        paths = [[(-15, -8), (-6, -8), (-6, 3), (5, 3), (5, -6), (14, -6)],
                 [(-15, -2), (-10, -2), (-10, 9), (2, 9), (2, 0), (14, 0)],
                 [(-15, 4), (-3, 4), (-3, -10), (9, -10), (9, 6), (14, 6)],
                 [(-15, 9), (0, 9), (0, -4), (11, -4), (11, 10), (14, 10)]]
        for i, (c, p) in enumerate(zip(COL, paths)):
            z = 2 * i
            for (xa, ya), (xb, yb) in zip(p, p[1:]):
                # one straight segment of this cable
                box(V, min(xa, xb), max(xa, xb) + 1, min(ya, yb), max(ya, yb) + 1, z, z + 1, c)
            # connectors at both ends
            box(V, -19, -16, p[0][1], p[0][1] + 1, z, z + 1, 'plastic_black')
            box(V, 15, 18, p[-1][1], p[-1][1] + 1, z, z + 1, 'plastic_black')
            box(V, -19, -19, p[0][1], p[0][1] + 1, z, z + 1, 'metal')
            box(V, 18, 18, p[-1][1], p[-1][1] + 1, z, z + 1, 'metal')
        # one loose tie and a label
        box(V, -15, -14, -9, 10, 8, 8, 'plastic_white')
        box(V, -13, -12, 9, 10, 7, 7, 'label')
    return V, C
R = build_states('CableBundle', model, ['Tidy', 'Messy'], size=FINE_SIZE, grain=0, collider=False, metallic=('metal',))
result = {s: {k: r[k] for k in ('object', 'voxels', 'height_m', 'warnings')} for s, r in R.items()}
