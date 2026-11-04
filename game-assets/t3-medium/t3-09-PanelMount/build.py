# PanelMount (t3-09) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: PanelMount
#   id: t3-09
#   tier: T3
#   room: Roof
#   priority: V1
#   size_cm: 110x60x70
#   voxels: 141x77x90
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # PanelMount
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel PanelMount for the Roof room, about 110x60x70 cm (141x77x90 voxels). Main forms: legs, rails.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: bolts, braces, clamps, rust, feet.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] legs
#   - [2] rails
#   - [3] bolts
#   - [3] braces
#   - [3] clamps
#   - [3] rust
#   - [3] feet

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t3-medium/t3-09-PanelMount'
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
# 3. MODEL: PanelMount
# ======================================================================
C = {**PALETTE}

# DESCRIPTION: Solar panel mount: 110x60x70 cm sloped steel rack with legs, rails, braces, clamps and rust.
# UNITS: 1 voxel = 1/128 m (F); x right, y back, z up; 141x77x90 voxels
S, F = VOXEL_SIZE, FINE_SIZE

def mx(V, x0, x1, y0, y1, z0, z1, k):
    # box on both sides of x=0
    box(V, x0, x1, y0, y1, z0, z1, k); box(V, -x1, -x0, y0, y1, z0, z1, k)

def mount():
    V = Grid()
    # front legs
    mx(V, 58, 63, -36, -33, 2, 45, 'metal')
    # rear legs
    mx(V, 58, 63, 33, 36, 2, 89, 'metal')
    # feet
    mx(V, 56, 65, -38, -31, 0, 1, 'rubber'); mx(V, 56, 65, 31, 38, 0, 1, 'rubber')
    # base tie bars
    mx(V, 59, 62, -34, 34, 12, 14, 'metal_dark')
    # diagonal braces
    for y in range(-30, 10):
        z = 15 + int((y + 30) * 1.4); mx(V, 59, 62, y, y, z, z + 2, 'metal_dark')
    # front cross rail
    box(V, -66, 66, -37, -34, 42, 45, 'metal_dark')
    # rear cross rail
    box(V, -66, 66, 33, 36, 86, 89, 'metal_dark')
    # sloped panel rails
    for y in range(-36, 37):
        z = 42 + int((y + 36) * 44 / 72); mx(V, 38, 42, y, y, z, z + 3, 'metal')
    # clamps
    for y in (-20, 20):
        z = 42 + int((y + 36) * 44 / 72); mx(V, 36, 44, y - 2, y + 2, z - 1, z + 4, 'trim')
    # bolts
    mx(V, 59, 60, -37, -37, 40, 41, 'metal_dark'); mx(V, 59, 60, 37, 37, 84, 85, 'metal_dark')
    # rust
    mx(V, 58, 59, -36, -33, 6, 14, 'rust'); mx(V, 58, 59, 33, 36, 6, 14, 'rust')
    return V
result = build('PanelMount', mount(), C, size=F, grain=0)
