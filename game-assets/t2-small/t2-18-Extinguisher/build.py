# Extinguisher (t2-18) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: Extinguisher
#   id: t2-18
#   tier: T2
#   room: Basement
#   priority: V1
#   size_cm: 16x16x55
#   voxels: 20x20x70
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # Extinguisher
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel Extinguisher for the Basement room, about 16x16x55 cm (20x20x70 voxels). Main forms: body, hose, handle.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: pin, gauge, label, nozzle, strap.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] body 14 x 14 x 37 cm round, warning_red
#   - [2] hose 1.6 cm along the right side, rubber
#   - [2] head + handle 6 x 5 x 8 cm, metal / metal_dark
#   - [3] safety pin 1.6 x 0.8 cm, warning_yellow
#   - [3] pressure gauge 2.4 cm, glass in metal_dark bezel
#   - [3] label 4.7 x 0.8 x 12 cm, label + label_text
#   - [3] nozzle 1.6 x 3 cm, metal_dark
#   - [3] strap ring 1.6 cm, rubber

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-18-Extinguisher'
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
# 3. MODEL: Extinguisher
# ======================================================================
# DESCRIPTION: Red fire extinguisher: round body with label, strap ring, brass-grey valve head, gauge, pin, lever handle and hose with nozzle. 16x16x55 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x/y centred, z = up. Circles are stepped rows.
import math
def circle(V, r, z0, z1, key):
    # stepped disc of radius r, layers z0..z1
    for iy in range(-r, r):
        w = int(math.sqrt(r * r - (iy + .5) ** 2) + .5)
        box(V, -w, w - 1, iy, iy, z0, z1, key)
V = Grid()
# rubber foot ring
circle(V, 9, 0, 1, 'rubber')
# red body
circle(V, 9, 2, 52, 'warning_red')
# strap ring around the lower body
circle(V, 9, 14, 15, 'chassis_dark')
# stepped shoulder
circle(V, 8, 53, 54, 'warning_red')
circle(V, 6, 55, 56, 'warning_red')
# metal neck
circle(V, 3, 57, 60, 'metal')
# valve head block
box(V, -3, 2, -3, 2, 61, 65, 'metal')
# label on the front of the body
box(V, -3, 2, -9, -9, 22, 40, 'label')
# label text lines
tile(V, -2, 1, -9, -9, 26, 26, 'label_text', nz=6, dz=2)
# gauge: bezel and glass on the front of the head
box(V, -2, 1, -4, -4, 61, 64, 'metal_dark')
box(V, -1, 0, -4, -4, 62, 63, 'glass')
# fixed handle over the head
box(V, -7, 2, -1, 0, 68, 69, 'metal_dark')
box(V, -7, -6, -1, 0, 66, 67, 'metal_dark')
# squeeze lever under the handle
box(V, -7, 4, -1, 0, 66, 66, 'metal_dark')
# safety pin through the head
box(V, 3, 5, 0, 0, 66, 67, 'warning_yellow')
# hose leaves the head and runs down the right side
box(V, 3, 10, -1, 0, 62, 63, 'rubber')
box(V, 9, 10, -1, 0, 21, 63, 'rubber')
# nozzle at the end of the hose
box(V, 9, 10, -1, 0, 17, 20, 'metal_dark')
R = build('Extinguisher', V, PALETTE, size=FINE_SIZE, grain=0, metallic=('metal',))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
