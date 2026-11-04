# PipeStraight (t3-03) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: PipeStraight
#   id: t3-03
#   tier: T3
#   room: Basement
#   priority: V1
#   size_cm: 8x100x8
#   voxels: 10x128x10
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # PipeStraight
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel PipeStraight for the Basement room, about 8x100x8 cm (10x128x10 voxels). Main forms: pipe, flanges.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: bolts, clamps, flow arrow, rust.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] pipe 6.2 cm dia (8 voxels) x 100 cm, trim
#   - [2] flanges 2x 7.8 cm dia x 2.3 cm, metal_dark
#   - [3] bolts 8x 0.8 cm on the flange faces, metal
#   - [3] clamps 2x 7.8 cm dia x 1.6 cm, rubber
#   - [3] flow arrow 1.6 x 12 cm on top, warning_yellow
#   - [3] rust patches 3x, rust

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t3-medium/t3-03-PipeStraight'
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
# 3. MODEL: PipeStraight
# ======================================================================
# DESCRIPTION: Straight steel pipe, 1 m long, flanged at both ends with bolts, two rubber clamps, a yellow flow arrow and rust patches. 8x100x8 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x/z cross-section 10x10 (octagonal tube), y = length (arrow points +y).
PIPE = [4, 6, 8, 8, 8, 8, 6, 4]            # tube width per z layer (z 1..8)
FLG = [6, 8, 10, 10, 10, 10, 10, 10, 8, 6]  # flange / clamp width per z layer (z 0..9)
def run_y(V, y0, y1, xc, prof, z0, key):
    # tube along y centred on x = xc (+0.5)
    for i, w in enumerate(prof):
        box(V, xc - w // 2 + 1, xc + w // 2, y0, y1, z0 + i, z0 + i, key)
V = Grid()
# main pipe
run_y(V, -64, 63, -1, PIPE, 1, 'trim')
# flanges at both ends
run_y(V, -64, -62, -1, FLG, 0, 'metal_dark')
run_y(V, 61, 63, -1, FLG, 0, 'metal_dark')
# two rubber clamps
run_y(V, -30, -29, -1, FLG, 0, 'rubber')
run_y(V, 28, 29, -1, FLG, 0, 'rubber')
# rust patches on the sides and top
box(V, -4, -4, -44, -38, 3, 6, 'rust')
box(V, 3, 3, 10, 18, 2, 5, 'rust')
box(V, -1, 0, 36, 44, 8, 8, 'rust')
# yellow flow arrow on top: shaft plus arrow head pointing +y
box(V, -1, 0, -14, 2, 8, 8, 'warning_yellow')
box(V, -2, 1, 3, 3, 8, 8, 'warning_yellow')
# bolts on the flange end faces
for (x, z) in [(-4, 2), (3, 2), (-4, 7), (3, 7)]:
    box(V, x, x, -64, -64, z, z, 'metal')
    box(V, x, x, 63, 63, z, z, 'metal')
R = build('PipeStraight', V, PALETTE, size=FINE_SIZE, grain=0, metallic=('metal', 'trim', 'metal_dark'))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
