# SuppressionTank (t4-03) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: SuppressionTank
#   id: t4-03
#   tier: T4
#   room: Basement
#   priority: UP
#   size_cm: 30x30x170
#   voxels: 19x19x109
#   grid: S
#   grain: 0
#   collider: false
#   ---
#
#   # SuppressionTank
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel SuppressionTank for the Basement room, about 30x30x170 cm (19x19x109 voxels). Main forms: cylinder, valve.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: gauge, hose, band, label, bracket.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] cylinder 28 x 28 x 140 cm stepped round, warning_red (dome shoulders + metal_dark foot ring)
#   - [2] valve neck + head 9 x 9 x 15 cm, copper; handwheel metal_dark
#   - [3] pressure gauge 6 cm on the neck, plastic_white in metal bezel
#   - [3] hose 3 cm along the right side to a nozzle, rubber / metal_dark
#   - [3] bands 2x warning_yellow rings; label 9 x 25 cm, label
#   - [3] wall bracket plates 2x on the back, metal_dark

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t4-large/t4-03-SuppressionTank'
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
# 3. MODEL: SuppressionTank
# ======================================================================
# DESCRIPTION: Tall red gas fire-suppression cylinder with brass valve and handwheel, pressure gauge, hose and nozzle, yellow bands, label and wall bracket plates. 30x30x170 cm.
# UNITS: VOXEL_SIZE (1.56 cm) voxels; x/y centred, z = up. Circles are stepped rows.
import math
def circle(V, r, z0, z1, key):
    # stepped disc of radius r, layers z0..z1
    for iy in range(-r, r):
        w = int(math.sqrt(r * r - (iy + .5) ** 2) + .5)
        box(V, -w, w - 1, iy, iy, z0, z1, key)
V = Grid()
# dark foot ring
circle(V, 9, 0, 2, 'metal_dark')
# red cylinder body
circle(V, 9, 3, 88, 'warning_red')
# stepped dome shoulders
circle(V, 8, 89, 90, 'warning_red')
circle(V, 6, 91, 92, 'warning_red')
# brass neck and valve head
circle(V, 3, 93, 98, 'copper')
box(V, -4, 3, -3, 2, 99, 104, 'copper')
# handwheel on top of the valve
box(V, -6, 5, -1, 0, 105, 106, 'metal_dark')
box(V, -1, 0, -6, 5, 105, 106, 'metal_dark')
# wheel screw
box(V, -1, 0, -1, 0, 107, 108, 'metal')
# gauge: bezel and white face on the front of the neck
box(V, -2, 1, -4, -4, 96, 99, 'metal_dark')
box(V, -1, 0, -4, -4, 97, 98, 'plastic_white')
# hose from valve, down the right side, with a nozzle
box(V, 4, 10, 0, 1, 101, 102, 'rubber')
box(V, 9, 10, 0, 1, 56, 102, 'rubber')
box(V, 9, 10, 0, 1, 50, 55, 'metal_dark')
# two yellow warning bands
circle(V, 9, 24, 25, 'warning_yellow')
circle(V, 9, 66, 67, 'warning_yellow')
# paper label and text lines on the front
box(V, -3, 2, -9, -9, 34, 60, 'label')
tile(V, -2, 1, -9, -9, 38, 38, 'label_text', nz=8, dz=3)
# wall bracket plates behind the cylinder
box(V, -7, 6, 9, 10, 14, 20, 'metal_dark')
box(V, -7, 6, 9, 10, 74, 80, 'metal_dark')
R = build('SuppressionTank', V, PALETTE, size=VOXEL_SIZE, grain=0, metallic=('copper', 'metal'))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
