# PipeCorner (t2-09) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: PipeCorner
#   id: t2-09
#   tier: T2
#   room: Basement
#   priority: V1
#   size_cm: 40x40x8
#   voxels: 51x51x10
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # PipeCorner
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel PipeCorner for the Basement room, about 40x40x8 cm (51x51x10 voxels). Main forms: pipe, flanges.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: bolts, clamps, weld seam.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] pipe L-elbow 6.2 cm dia (8 voxels), trim
#   - [2] flanges 2x 7.8 cm dia x 2.3 cm, metal_dark
#   - [3] bolts 8x 0.8 cm on the flange faces, metal
#   - [3] clamps 2x 7.8 cm dia x 1.6 cm, rubber
#   - [3] weld seam 2x ring at the elbow, seam

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-09-PipeCorner'
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
# 3. MODEL: PipeCorner
# ======================================================================
# DESCRIPTION: 90-degree steel pipe elbow lying flat, flanged at both open ends, with bolts, clamps and weld rings. 40x40x8 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x/y footprint 51x51, z = up. Pipe is an octagon-section tube lying along x then y.
PIPE = [4, 6, 8, 8, 8, 8, 6, 4]            # tube width per z layer (z 1..8)
FLG = [6, 8, 10, 10, 10, 10, 10, 10, 8, 6]  # flange / clamp width per z layer (z 0..9)
def run_x(V, x0, x1, yc, prof, z0, key):
    # tube along x centred on y = yc (+0.5)
    for i, w in enumerate(prof):
        box(V, x0, x1, yc - w // 2 + 1, yc + w // 2, z0 + i, z0 + i, key)
def run_y(V, y0, y1, xc, prof, z0, key):
    # tube along y centred on x = xc (+0.5)
    for i, w in enumerate(prof):
        box(V, xc - w // 2 + 1, xc + w // 2, y0, y1, z0 + i, z0 + i, key)
V = Grid()
# pipe leg along x (front), centre y = -21
run_x(V, -25, 24, -21, PIPE, 1, 'trim')
# pipe leg along y (right side), centre x = 21
run_y(V, -24, 25, 21, PIPE, 1, 'trim')
# end flanges: left end of the x leg, back end of the y leg
run_x(V, -25, -23, -21, FLG, 0, 'metal_dark')
run_y(V, 23, 25, 21, FLG, 0, 'metal_dark')
# clamps on each leg
run_x(V, -14, -13, -21, FLG, 0, 'rubber')
run_y(V, 10, 11, 21, FLG, 0, 'rubber')
# weld seam rings near the elbow
run_x(V, 12, 12, -21, PIPE, 1, 'seam')
run_y(V, -12, -12, 21, PIPE, 1, 'seam')
# bolts on the flange faces
for (y, z) in [(-25, 2), (-17, 2), (-25, 7), (-17, 7)]:
    box(V, -25, -25, y, y, z, z, 'metal')
for (x, z) in [(17, 2), (25, 2), (17, 7), (25, 7)]:
    box(V, x, x, 25, 25, z, z, 'metal')
R = build('PipeCorner', V, PALETTE, size=FINE_SIZE, grain=0, metallic=('metal', 'trim', 'metal_dark'))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
