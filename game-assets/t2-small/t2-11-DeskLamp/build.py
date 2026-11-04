# DeskLamp (t2-11) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: DeskLamp
#   id: t2-11
#   tier: T2
#   room: Control
#   priority: V1
#   size_cm: 20x20x42
#   voxels: 26x26x54
#   grid: F
#   grain: 0
#   collider: false
#   states:
#     On: bulb in a bright warm key, inside of the shade lit
#     Off: bulb in a dull grey key
#   ---
#
#   # DeskLamp
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel DeskLamp for the Control room, about 20x20x42 cm (26x26x54 voxels). Main forms: arm, shade, base.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: switch, hinge joints, cable, bulb.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] arm: 2 x 2 x 30 cm, metal
#   - [2] shade: 14 x 12 x 12 cm, chassis
#   - [2] base: 12 x 12 x 3 cm, chassis_dark
#   - [3] switch: 2 x 1 x 1 cm, trim
#   - [3] hinge joints: 4 x 4 cm, metal_dark
#   - [3] cable: 1 cm, rubber
#   - [3] bulb: 3 x 3 x 3 cm, led_yellow

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-11-DeskLamp'
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
# 3. MODEL: DeskLamp
# ======================================================================
C = {**PALETTE}
# DESCRIPTION: Articulated desk lamp with a stepped square shade, base switch and cable; On has a lit bulb. 20x20x42 cm.
# UNITS: FINE_SIZE (7.8 mm); x = width (26), y = depth, z = up (54).
def model(state):
    V = Grid()
    on = state == 'On'
    # heavy base and step
    box(V, -12, -1, -6, 5, 0, 2, 'chassis_dark')
    box(V, -10, -3, -4, 3, 3, 3, 'chassis_dark')
    # base switch on the front
    box(V, -8, -6, -7, -7, 1, 2, 'trim')
    # cable lying behind the lamp
    box(V, -9, -8, 6, 12, 0, 1, 'rubber')
    # lower arm and lower hinge
    box(V, -7, -6, -1, 0, 4, 28, 'metal')
    box(V, -8, -5, -2, 1, 28, 31, 'metal_dark')
    # stepped upper arm rising to the shade
    for i in range(9):
        box(V, -6 + i, -6 + i + 1, -1, 0, 30 + round(i * 1.6), 31 + round(i * 1.6), 'metal')
    # head hinge
    box(V, 2, 5, -2, 1, 43, 46, 'metal_dark')
    # stepped square shade: outer shell, lit lining, hollow inside
    layers = [(42, 45, 6), (46, 48, 5), (49, 51, 4), (52, 53, 3)]
    for z0, z1, r in layers:
        box(V, 7 - r, 6 + r, -r, r - 1, z0, z1, 'chassis')
    for z0, z1, r in layers:
        if z0 <= 51:
            box(V, 7 - r + 1, 6 + r - 1, -r + 1, r - 2, z0, min(z1, 51), 'led_yellow' if on else 'trim')
    hole = Grid()
    for z0, z1, r in layers:
        if z0 <= 51:
            box(hole, 7 - r + 2, 6 + r - 2, -r + 2, r - 3, z0, min(z1, 51), 'chassis')
    carve(V, hole)
    # bulb inside the shade
    box(V, 6, 7, -1, 0, 44, 47, 'led_yellow' if on else 'metal')
    return V, C
res = build_states('DeskLamp', model, ['On', 'Off'], size=FINE_SIZE, grain=0, metallic=('metal',))
result = {s: {k: r[k] for k in ('object', 'voxels', 'height_m', 'warnings')} for s, r in res.items()}
