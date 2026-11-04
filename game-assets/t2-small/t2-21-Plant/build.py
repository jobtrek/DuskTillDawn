# Plant (t2-21) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: Plant
#   id: t2-21
#   tier: T2
#   room: Control
#   priority: V1
#   size_cm: 30x30x60
#   voxels: 38x38x77
#   grid: F
#   grain: 0.08
#   collider: false
#   ---
#
#   # Plant
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel Plant for the Control room, about 30x30x60 cm (38x38x77 voxels). Main forms: pot, stem.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.08.
#
#   ### Details
#   Add the small features: leaves, soil, rim, saucer.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] pot: 24 x 24 x 18 cm, rust
#   - [2] stem: 2 x 2 cm stems, stem
#   - [3] leaves (blob ok, organic): 14 cm blobs, leaf
#   - [3] soil: 20 x 20 cm, wood_dark
#   - [3] rim: 28 x 28 x 3 cm, copper
#   - [3] saucer: 30 x 30 x 1.5 cm, metal_dark

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-21-Plant'
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
# 3. MODEL: Plant
# ======================================================================
C = {**PALETTE, 'stem': (0.10, 0.35, 0.08), 'leaf': (0.08, 0.50, 0.10), 'leaf_dark': (0.04, 0.30, 0.07)}
# DESCRIPTION: Potted green plant with terracotta pot, saucer, soil and five leafy stems. 30x30x60 cm.
# UNITS: FINE_SIZE (7.8 mm); x,y = width (38), z = up (77).
V = Grid()
# saucer
box(V, -13, 12, -13, 12, 0, 1, 'metal_dark')
# pot body, narrower at the bottom
box(V, -8, 7, -8, 7, 2, 3, 'rust')
box(V, -10, 9, -10, 9, 4, 19, 'rust')
# pot rim
box(V, -12, 11, -12, 11, 20, 23, 'copper')
# soil in the pot
box(V, -9, 8, -9, 8, 23, 23, 'wood_dark')
# five stems with a leaf cluster at each tip
tips = [(0, 0, 70), (-10, -5, 58), (11, 4, 54), (-6, 10, 46), (7, -10, 48)]
for tx, ty, tz in tips:
    n = tz - 23
    for s in range(n):
        x, y = round(tx * s / n), round(ty * s / n)
        box(V, x, x + 1, y, y + 1, 23 + s, 23 + s, 'stem')
    # leaf cluster
    blob(V, tx, ty, tz, 8, 7, 4, 'leaf')
    blob(V, tx, ty, tz - 3, 5, 5, 2, 'leaf_dark')
R = build('Plant', V, C, size=FINE_SIZE, grain=0.08)
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
