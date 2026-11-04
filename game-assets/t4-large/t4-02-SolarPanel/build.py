# SolarPanel (t4-02) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: SolarPanel
#   id: t4-02
#   tier: T4
#   room: Roof
#   priority: V1
#   size_cm: 100x4x165
#   voxels: 128x5x211
#   grid: F
#   grain: 0
#   collider: false
#   states:
#     Clean: evenly blue cells, bright glass
#     Dirty: dust patches in a pale brown key, a few leaf voxels, duller glass
#     Damaged: diagonal crack lines across 3–4 cells, one missing cell corner, frame bent by 1–2 voxels
#   ---
#
#   # SolarPanel
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel SolarPanel for the Roof room, about 100x4x165 cm (128x5x211 voxels). Main forms: frame, glass.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: 60 cells, busbars, junction box, corner keys, label.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] frame
#   - [2] glass
#   - [3] 60 cells (tile)
#   - [3] busbars
#   - [3] junction box
#   - [3] corner keys
#   - [3] label

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t4-large/t4-02-SolarPanel'
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
# 3. MODEL: SolarPanel
# ======================================================================
C = {**PALETTE, 'cell_blue': (0.04, 0.10, 0.35), 'cell_dull': (0.06, 0.10, 0.22),
     'glass_dull': (0.30, 0.38, 0.42), 'dust': (0.55, 0.45, 0.30)}

# DESCRIPTION: Solar panel: 100x165 cm aluminium frame, 60 blue cells (6x10) with busbars, junction box and label on the back; Clean/Dirty/Damaged.
# UNITS: 1 voxel = 1/128 m (F); x right, y back (-y front), z up; 128x6x211 voxels

def model(state):
    V = Grid()
    bg = 'glass_dull' if state in ('Dirty', 'Dirty2') else 'glass'
    cell = 'cell_dull' if state in ('Dirty', 'Dirty2') else 'cell_blue'
    # aluminium frame, solid slab
    box(V, -64, 63, -2, 1, 0, 210, 'metal')
    # recessed glass plane inside the frame
    box(V, -61, 60, -2, -2, 3, 207, 'seam')
    box(V, -61, 60, -1, -1, 3, 207, bg)
    # back sheet
    box(V, -61, 60, 1, 1, 3, 207, 'plastic_white')
    # 6 x 10 = 60 solar cells
    tile(V, -59, -41, -1, -1, 6, 24, cell, nx=6, nz=10, dx=20, dz=20)
    # three busbars per cell
    for off in (5, 9, 13):
        tile(V, -59 + off, -59 + off, -2, -2, 6, 24, 'trim', nx=6, nz=10, dx=20, dz=20)
    # corner keys
    tile(V, -64, -60, -2, 1, 0, 4, 'metal_dark', nx=2, nz=2, dx=123, dz=206)
    # junction box and bypass leads on the back
    box(V, -8, 7, 2, 3, 150, 160, 'plastic_black')
    box(V, -1, 0, 2, 3, 140, 149, 'rubber')
    # rating label on the back
    box(V, 30, 55, 2, 2, 8, 20, 'label')
    tile(V, 33, 52, 2, 2, 10, 10, 'label_text', nz=4, dz=3)
    if state == 'Dirty':
        # dust patches along the bottom and left, a few leaves
        for x0, x1, z0, z1 in ((-61, -35, 3, 12), (-5, 60, 3, 8), (-61, -56, 3, 60), (40, 60, 3, 30), (-30, -10, 100, 106)):
            box(V, x0, x1, -2, -2, z0, z1, 'dust')
        for x0, z0, k in ((10, 6, 'wood'), (30, 5, 'wood_dark'), (-40, 13, 'wood')):
            box(V, x0, x0 + 2, -2, -2, z0, z0 + 1, k)
    if state == 'Dirty2':
        # heavy dust band along the bottom with a ragged top edge
        box(V, -61, 60, -2, -2, 3, 22, 'dust')
        tile(V, -61, -54, -2, -2, 23, 27, 'dust', nx=10, dx=12)
        # drip streaks running down from the dust band
        for x0, z1 in ((-50, 90), (-22, 60), (6, 110), (34, 75), (52, 50)):
            box(V, x0, x0 + 2, -2, -2, 28, z1, 'dust')
        # bird droppings
        for x0, z0 in ((-30, 150), (14, 175), (44, 130)):
            box(V, x0, x0 + 3, -2, -2, z0, z0 + 2, 'plastic_white')
        # leaves stuck in the dust
        for x0, z0, k in ((-40, 8, 'wood'), (-5, 12, 'wood_dark'), (20, 6, 'wood'), (48, 14, 'wood_dark')):
            box(V, x0, x0 + 3, -2, -2, z0, z0 + 1, k)
    if state == 'Damaged':
        # diagonal cracks over 3-4 cells
        for i in range(60):
            box(V, -50 + i, -50 + i, -2, -2, 60 + int(i * 1.2), 60 + int(i * 1.2), 'chassis_dark')
            box(V, 20 - i // 2, 20 - i // 2, -2, -2, 130 + i, 130 + i, 'chassis_dark')
        # one cell corner missing
        R = Grid(); box(R, 33, 39, -2, -1, 118, 124, 'x'); carve(V, R)
        box(V, 33, 39, -1, -1, 118, 124, 'plastic_white')
        # frame bent outward by 1 voxel on the right edge
        R = Grid(); box(R, 60, 63, -2, 1, 100, 130, 'x'); carve(V, R)
        box(V, 61, 64, -2, 1, 100, 130, 'metal')
    return V, C

result = build_states('SolarPanel', model, ['Clean', 'Dirty', 'Dirty2', 'Damaged'], size=FINE_SIZE, grain=0)
