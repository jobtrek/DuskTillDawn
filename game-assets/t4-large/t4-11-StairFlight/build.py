# StairFlight (t4-11) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: StairFlight
#   id: t4-11
#   tier: T4
#   room: Between floors
#   priority: V1
#   size_cm: 100x225x140
#   voxels: 64x144x90
#   grid: S
#   grain: 0
#   collider: true
#   ---
#
#   # StairFlight
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel StairFlight for the Between floors room, about 100x225x140 cm (64x144x90 voxels). Main forms: treads, stringers, railing.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: nosing strips, anti-slip, balusters, bolts, wear.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] treads: 58 x 20 x 2 cm, metal
#   - [2] stringers: 3 x 225 x var cm, chassis
#   - [2] railing: 3 x 225 x 3 cm, trim
#   - [3] nosing strips: 58 x 3 cm, warning_yellow
#   - [3] anti-slip: 54 x 1.5 cm, metal_dark
#   - [3] balusters: 3 x 3 x 40 cm, metal_dark
#   - [3] bolts: 1.5 cm, metal
#   - [3] wear: 3 cm patches, rust

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t4-large/t4-11-StairFlight'
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
# 3. MODEL: StairFlight
# ======================================================================
C = {**PALETTE}
# DESCRIPTION: Straight steel stair flight of 7 steps with side stringers, handrails, balusters and yellow nosings. About 100x225x94 cm rise plus rails.
# UNITS: VOXEL_SIZE (1.56 cm); x = width (64), y = length (144, climbs toward +y), z = up (90 incl. rails).
V = Grid()
N = 7
prev = -1
for i in range(N):
    y0 = -72 + round(144 * i / N)
    y1 = -72 + round(144 * (i + 1) / N) - 1
    zt = round(60 * (i + 1) / N) - 1
    # solid side stringers (stepped profile)
    tile(V, -32, -30, y0, y1, 0, zt, 'chassis', nx=2, dx=62)
    # riser plate at the front of the step
    box(V, -29, 28, y0, y0, prev + 1, zt, 'metal_dark')
    # tread slab
    box(V, -29, 28, y0, y1, zt - 2, zt, 'metal')
    # yellow nosing strip
    box(V, -29, 28, y0, y0 + 2, zt, zt, 'warning_yellow')
    # anti-slip lines
    tile(V, -27, 26, y0 + 6, y0 + 6, zt, zt, 'metal_dark', ny=3, dy=4)
    # balusters on both sides
    tile(V, -32, -31, y0 + 2, y0 + 3, zt + 1, zt + 27, 'metal_dark', nx=2, dx=62)
    # stepped handrail following the stairs
    tile(V, -32, -30, y0, y1, zt + 28, zt + 29, 'trim', nx=2, dx=62)
    # bolts on the stringer outer faces
    tile(V, -32, -32, y0 + 8, y0 + 8, zt - 4, zt - 4, 'metal', nx=2, dx=63)
    prev = zt
# rust wear patches on some treads
box(V, -20, -17, -50, -47, 6, 6, 'rust')
box(V, 8, 12, 5, 8, 33, 33, 'rust')
box(V, -10, -7, 45, 48, 52, 52, 'rust')
R = build('StairFlight', V, C, size=VOXEL_SIZE, grain=0, collider=True, metallic=('metal', 'metal_dark', 'trim'))
result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}
