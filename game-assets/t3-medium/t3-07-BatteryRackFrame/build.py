# BatteryRackFrame (t3-07) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: BatteryRackFrame
#   id: t3-07
#   tier: T3
#   room: Basement
#   priority: V1
#   size_cm: 60x80x100
#   voxels: 77x102x128
#   grid: F
#   grain: 0
#   collider: true
#   ---
#
#   # BatteryRackFrame
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel BatteryRackFrame for the Basement room, about 60x80x100 cm (77x102x128 voxels). Main forms: posts, shelves, side panels.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: rail holes, cable entries, casters, label.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [1] posts 4x 2.3 x 2.3 x 94 cm, metal
#   - [2] shelves 6x 55 x 74 x 1.6 cm, trim
#   - [2] side panels 2x 1.6 x 74 x 91 cm, chassis
#   - [3] rail holes (tile) 1 x 2 voxels every 6 voxels on the front posts, seam
#   - [3] cable entries 2x 9 x 5 cm in the side panels, rubber + seam
#   - [3] casters 4x 3 x 3 x 5 voxels, rubber on metal_dark fork
#   - [3] label 16 x 4 cm on the top front rail, label

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t3-medium/t3-07-BatteryRackFrame'
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
# 3. MODEL: BatteryRackFrame
# ======================================================================
# DESCRIPTION: Open steel frame for battery modules: four posts, six shelves, grey side panels with cable grommets, rail holes, casters and a label. 60x80x100 cm.
# UNITS: FINE_SIZE (7.8 mm) voxels; x = width, y = depth (-y front), z = up.
V = Grid()
# four caster wheels with forks
tile(V, -38, -36, -51, -49, 0, 3, 'rubber', nx=2, ny=2, dx=74, dy=99)
tile(V, -38, -36, -51, -49, 4, 6, 'metal_dark', nx=2, ny=2, dx=74, dy=99)
# four corner posts
tile(V, -38, -36, -51, -49, 7, 127, 'metal', nx=2, ny=2, dx=74, dy=99)
# side panels
box(V, -38, -37, -48, 47, 8, 125, 'chassis')
box(V, 37, 38, -48, 47, 8, 125, 'chassis')
# six shelves
tile(V, -35, 35, -48, 47, 10, 11, 'trim', nz=6, dz=22)
# top front rail and label
box(V, -35, 35, -51, -50, 122, 127, 'metal')
box(V, -10, 10, -51, -51, 123, 126, 'label')
# rail holes on the two front posts
tile(V, -37, -37, -51, -51, 14, 15, 'seam', nz=18, dz=6)
tile(V, 37, 37, -51, -51, 14, 15, 'seam', nz=18, dz=6)
# cable entries in both side panels: dark hole with rubber grommet
for x in (-38, 38):
    box(V, x, x, -6, 5, 40, 45, 'rubber')
    box(V, x, x, -4, 3, 41, 44, 'seam')
R = build('BatteryRackFrame', V, PALETTE, size=FINE_SIZE, grain=0, collider=True, metallic=('metal', 'trim'))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
