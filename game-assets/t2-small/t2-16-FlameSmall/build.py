# FlameSmall (t2-16) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: FlameSmall
#   id: t2-16
#   tier: T2
#   room: Basement
#   priority: V1
#   size_cm: 30x30x50
#   voxels: 19x19x32
#   grid: S
#   grain: 0
#   collider: false
#   states:
#     A: flame tips leaning left
#     B: flame tips leaning right (same base and height ±2 voxels, for flicker)
#   ---
#
#   # FlameSmall
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel FlameSmall for the Basement room, about 30x30x50 cm (19x19x32 voxels). Main forms: 3 flame layers.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: hot core, stepped tips, ember voxels at the base.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] flame outer layer 30 x 30 x 50 cm stepped, led_red
#   - [2] flame mid layer, led_orange
#   - [2] flame inner layer, led_yellow
#   - [3] hot core 3 x 3 x 9 cm, plastic_white
#   - [3] stepped tips, 1 voxel wide (_A leans left, _B leans right)
#   - [3] ember voxels 4x at the base, led_yellow

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-16-FlameSmall'
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
# 3. MODEL: FlameSmall
# ======================================================================
# DESCRIPTION: Small stepped square flame in three nested layers (red, orange, yellow) with a white-hot core and embers. _A and _B lean opposite ways for a flicker. 30x30x50 cm.
# UNITS: VOXEL_SIZE (1.56 cm) voxels; x/y centred, z = up.
HW = [9, 9, 9, 8, 8, 8, 7, 7, 7, 6, 6, 6, 6, 5, 5, 5, 5, 4, 4, 4, 3, 3, 3, 2, 2, 2, 1, 1, 1, 0, 0, 0]
def model(state):
    V = Grid()
    lean = -1 if state == 'A' else 1
    for z, hw in enumerate(HW):
        # sideways lean grows towards the tip
        off = 0 if z < 12 else (lean if z < 22 else 2 * lean)
        # red outer layer
        box(V, off - hw, off + hw, -hw, hw, z, z, 'led_red')
        # orange middle layer
        if hw >= 2:
            box(V, off - hw + 2, off + hw - 2, -hw + 2, hw - 2, z, z, 'led_orange')
        # yellow inner layer
        if hw >= 4 and z <= 22:
            box(V, off - hw + 4, off + hw - 4, -hw + 4, hw - 4, z, z, 'led_yellow')
    # white-hot core at the base of the flame
    box(V, -1, 1, -1, 1, 3, 11, 'plastic_white')
    # four embers sitting at the base
    for (x, y, z) in [(-9, 2, 3), (9, -3, 4), (3, 9, 3), (-4, -9, 4)]:
        box(V, x, x, y, y, z, z, 'led_yellow')
    return V, PALETTE
R = build_states('FlameSmall', model, ['A', 'B'], size=VOXEL_SIZE, grain=0, collider=False)
result = {s: {k: r[k] for k in ('object', 'voxels', 'height_m', 'warnings')} for s, r in R.items()}
