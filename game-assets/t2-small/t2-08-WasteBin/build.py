# WasteBin (t2-08) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: WasteBin
#   id: t2-08
#   tier: T2
#   room: Control
#   priority: V1
#   size_cm: 25x25x30
#   voxels: 32x32x38
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # WasteBin
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel WasteBin for the Control room, about 25x25x30 cm (32x32x38 voxels). Main forms: rim, liner.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: ridges, crumpled paper, label.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] rim: 32 x 32 x 4 cm, trim
#   - [2] liner: 1 cm inner skin, plastic_black
#   - [3] ridges: 1 cm bands, chassis_dark
#   - [3] crumpled paper: 5 cm balls, label
#   - [3] label: 6 x 5 cm, warning_yellow

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-08-WasteBin'
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
# 3. MODEL: WasteBin
# ======================================================================
C = {**PALETTE}
# DESCRIPTION: Round-ish grey office waste bin with ridges, black liner, crumpled paper and a label. 25x25x30 cm.
# UNITS: FINE_SIZE (7.8 mm); x,y = width (32), z = up (38).
def ring(V, a, b, z0, z1, k):
    box(V, a, b, a, a, z0, z1, k); box(V, a, b, b, b, z0, z1, k)
    box(V, a, a, a, b, z0, z1, k); box(V, b, b, a, b, z0, z1, k)
V = Grid()
# floor of the bin and liner floor
box(V, -12, 11, -12, 11, 0, 1, 'chassis')
box(V, -10, 9, -10, 9, 2, 2, 'plastic_black')
# tapered wall: outer metal ring and inner liner ring per layer
for z in range(2, 34):
    r = 12 if z < 10 else 13 if z < 20 else 14
    ring(V, -r, r - 1, z, z, 'chassis')
    ring(V, -r + 1, r - 2, z, z, 'plastic_black')
# horizontal ridges
for z in (8, 17, 26):
    r = (12 if z < 10 else 13 if z < 20 else 14) + 1
    ring(V, -r, r - 1, z, z, 'chassis_dark')
# rim flange
ring(V, -16, 15, 34, 37, 'trim')
ring(V, -15, 14, 34, 37, 'trim')
ring(V, -14, 13, 34, 37, 'plastic_black')
# crumpled paper balls inside
blob(V, -4, 3, 6, 4, 4, 4, 'label')
blob(V, 4, -3, 5, 4, 4, 3, 'label')
blob(V, 0, 0, 32, 4, 4, 3, 'label')
# warning label on the front
box(V, -4, 3, -14, -14, 11, 15, 'warning_yellow')
R = build('WasteBin', V, C, size=FINE_SIZE, grain=0)
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
