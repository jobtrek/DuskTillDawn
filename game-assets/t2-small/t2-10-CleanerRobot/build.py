# CleanerRobot (t2-10) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: CleanerRobot
#   id: t2-10
#   tier: T2
#   room: Roof
#   priority: UP
#   size_cm: 30x40x15
#   voxels: 38x51x19
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # CleanerRobot
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel CleanerRobot for the Roof room, about 30x40x15 cm (38x51x19 voxels). Main forms: body, brush wheels.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: bumper, sensor, LED, handle.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] body
#   - [2] brush wheels
#   - [3] bumper
#   - [3] sensor
#   - [3] LED
#   - [3] handle

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-10-CleanerRobot'
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
# 3. MODEL: CleanerRobot
# ======================================================================
C = {**PALETTE}

# DESCRIPTION: Robot vacuum: 30x40x15 cm white body with bumper, front brush, drive wheels, sensor turret, LEDs and handle.
# UNITS: 1 voxel = 1/128 m (F); x right, y back, z up; 38x51x19 voxels
S, F = VOXEL_SIZE, FINE_SIZE

def mx(V, x0, x1, y0, y1, z0, z1, k):
    # box on both sides of x=0
    box(V, x0, x1, y0, y1, z0, z1, k); box(V, -x1, -x0, y0, y1, z0, z1, k)

def robot():
    V = Grid()
    # body
    box(V, -16, 15, -19, 24, 3, 13, 'plastic_white')
    # body chamfer
    box(V, -14, 13, -20, 25, 3, 13, 'plastic_white')
    # bumper
    box(V, -15, 14, -22, -20, 6, 12, 'chassis')
    # front brush roller
    box(V, -12, 11, -25, -23, 0, 5, 'fabric_dark')
    box(V, -13, -13, -25, -23, 0, 5, 'warning_yellow'); box(V, 12, 12, -25, -23, 0, 5, 'warning_yellow')
    # drive wheels
    mx(V, 17, 19, -4, 8, 0, 8, 'rubber')
    # wheel hubs
    mx(V, 19, 19, -1, 3, 2, 6, 'metal')
    # rear caster
    box(V, -2, 1, 20, 23, 0, 2, 'chassis_dark')
    # top deck
    box(V, -12, 11, -17, 16, 13, 13, 'chassis')
    # sensor turret
    box(V, -5, 4, 2, 11, 14, 17, 'chassis_dark')
    # sensor lens
    box(V, -3, 2, 1, 1, 15, 16, 'glass')
    # status LEDs
    tile(V, -6, -5, -12, -11, 14, 14, 'led_green', nx=3, dx=5)
    box(V, -1, 0, -12, -11, 14, 14, 'led_yellow')
    # rear handle
    mx(V, 5, 7, 18, 20, 14, 16, 'chassis_dark'); box(V, -8, 7, 18, 20, 17, 18, 'chassis_dark')
    return V
result = build('CleanerRobot', robot(), C, size=F, grain=0)
