# InverterBox (t2-20) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: InverterBox
#   id: t2-20
#   tier: T2
#   room: Roof
#   priority: V1
#   size_cm: 40x20x60
#   voxels: 51x26x77
#   grid: F
#   grain: 0
#   collider: false
#   states:
#     On: display lit, green LED
#     Off: display dark, all LEDs off
#     Broken: cracked display, red LED, scorch marks around the vents
#   ---
#
#   # InverterBox
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel InverterBox for the Roof room, about 40x20x60 cm (51x26x77 voxels). Main forms: housing, display.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: LEDs, heat fins, DC switch, cable glands, label.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] housing
#   - [2] display
#   - [3] LEDs
#   - [3] heat fins
#   - [3] DC switch
#   - [3] cable glands
#   - [3] label

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-20-InverterBox'
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
# 3. MODEL: InverterBox
# ======================================================================
C = {**PALETTE, 'disp_on': (0.08, 0.45, 0.38)}

# DESCRIPTION: Solar inverter box: 40x20x60 cm white housing, display, LEDs, heat fins, DC switch, glands, label; 3 states.
# UNITS: 1 voxel = 1/128 m (F); x right, y back, z up; 51x26x77 voxels
S, F = VOXEL_SIZE, FINE_SIZE

def inverter(state):
    V = Grid()
    # feet
    tile(V, -23, -18, -10, -4, 0, 1, 'rubber', nx=2, ny=2, dx=41, dy=12)
    # cable glands
    tile(V, -14, -12, -6, -4, 0, 0, 'plastic_black', nx=4, dx=9)
    tile(V, -14, -12, -6, -4, 1, 1, 'metal', nx=4, dx=9)
    # housing
    box(V, -22, 22, -12, 12, 2, 74, 'plastic_white')
    # top cap
    box(V, -22, 22, -12, 12, 75, 76, 'metal_dark')
    # heat fins R
    tile(V, 23, 25, -10, -10, 6, 68, 'metal', ny=6, dy=4)
    # heat fins L
    tile(V, -25, -23, -10, -10, 6, 68, 'metal', ny=6, dy=4)
    if state == 'Broken':                                                      # scorch marks
        box(V, -19, 19, -13, -13, 6, 19, 'wood_dark'); box(V, -12, 8, -13, -13, 20, 24, 'wood_dark')
    # vent slots
    tile(V, -18, 18, -13, -13, 8, 8, 'chassis_dark', nz=4, dz=3)
    # display bezel
    box(V, -15, 15, -13, -13, 44, 70, 'chassis_dark')
    # screen
    box(V, -13, 13, -13, -13, 46, 68, 'disp_on' if state == 'On' else 'screen')
    if state == 'On':
        for i in range(5): box(V, -10 + i * 5, -8 + i * 5, -13, -13, 50, 52 + 3 * i, 'led_green')
    if state == 'Broken':                                                      # cracks
        for i in range(22): box(V, -12 + i, -12 + i, -13, -13, 46 + i, 46 + i, 'trim')
        for i in range(12): box(V, 4 - i, 4 - i, -13, -13, 58 + i, 58 + i, 'trim')
        box(V, 6, 12, -13, -13, 46, 50, 'glass')
    on = {'On': ('led_green', 'led_off', 'led_off'), 'Off': ('led_off',) * 3,
          'Broken': ('led_off', 'led_off', 'led_red')}[state]
    # status LEDs
    for i, k in enumerate(on): box(V, -8 + 8 * i, -7 + 8 * i, -13, -13, 72, 73, k)
    # label
    box(V, -20, -6, -13, -13, 26, 36, 'label')
    tile(V, -18, -8, -13, -13, 28, 28, 'label_text', nz=3, dz=3)
    # DC switch
    box(V, 12, 20, -13, -13, 24, 32, 'plastic_black'); box(V, 14, 18, -13, -13, 26, 30, 'warning_red')
    return V, C
result = build_states('InverterBox', inverter, ['On', 'Off', 'Broken'], size=F, grain=0)
