# AntennaDish (t2-23) -- hand-written model; engine loaded from the SDK checkout (thin).
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P). Needs the repo checked out.
# To modify: edit ONLY the MODEL section at the bottom (and the palette extras above it).
# ---- spec.md (design intent) ----
#   ---
#   name: AntennaDish
#   id: t2-23
#   tier: T2
#   room: Roof
#   priority: V1
#   size_cm: 60x40x60
#   voxels: 77x51x77
#   grid: F
#   grain: 0
#   collider: false
#   ---
#
#   # AntennaDish
#
#   ## Prompt
#
#   ### Shape
#   Build a voxel AntennaDish for the Roof room, about 60x40x60 cm (77x51x77 voxels). Main forms: dish, feed horn, struts.
#
#   ### Materials
#   Use the colour keys in the Parts list, taken from `game-assets/palette.py`. Hard-edged blocks, grain 0.
#
#   ### Details
#   Add the small features: ribs, mount clamp, cable, label, bolts.
#
#   ## Parts (tier; cm size + colour key to fill before first build)
#
#   - [2] dish
#   - [2] feed horn
#   - [2] struts
#   - [3] ribs
#   - [3] mount clamp
#   - [3] cable
#   - [3] label
#   - [3] bolts
#
#   ## Notes
#
#   - Rotated in engine (replaces aligned / misaligned states)

import os
try:
    _here = os.path.dirname(os.path.abspath(__file__))
except NameError:
    _here = '/Users/jobtrekwork/DuskTillDawn/game-assets/t2-small/t2-23-AntennaDish'
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
# 3. MODEL: AntennaDish
# ======================================================================
C = {**PALETTE}

# DESCRIPTION: Satellite dish: 60x40x60 cm ribbed dish on a clamped mast, feed horn on three struts, cable and label.
# UNITS: 1 voxel = 1/128 m (F); x right, y back, z up; 77x51x77 voxels
S, F = VOXEL_SIZE, FINE_SIZE

def dish():
    V = Grid()
    cz = 40
    # paraboloid bowl, opens to -y
    for x in range(-36, 37):
        for z in range(-36, 37):
            r2 = x * x + z * z
            if r2 <= 36 * 36:
                ys = 2 - int(r2 / 92.6)
                box(V, x, x, ys, ys + 1, cz + z, cz + z, 'plastic_white')
    # ribs
    for a in range(8):
        t = a * math.pi / 4
        for r in range(3, 37):
            x, z = round(r * math.cos(t)), round(r * math.sin(t))
            y = 2 - int(r * r / 92.6) - 1
            box(V, x, x, y, y, cz + z, cz + z, 'trim')
    # rim ring
    for a in range(0, 360, 3):
        t = math.radians(a); x, z = round(36 * math.cos(t)), round(36 * math.sin(t))
        box(V, x, x, -13, -12, cz + z, cz + z, 'metal')
    # feed struts
    for a in (90, 210, 330):
        t = math.radians(a)
        for i in range(30):
            r = 34 - 31 * i / 29
            x, z = round(r * math.cos(t)), round(r * math.sin(t))
            y = -11 - round(13 * i / 29)
            box(V, x, x + 1, y, y, cz + z, cz + z + 1, 'metal_dark')
    # feed horn
    box(V, -3, 2, -25, -20, cz - 2, cz + 3, 'trim')
    box(V, -2, 1, -25, -25, cz - 1, cz + 2, 'chassis_dark')
    # base plate
    box(V, -8, 7, 1, 12, 0, 1, 'metal_dark')
    # mast
    box(V, -2, 1, 5, 8, 2, cz + 4, 'metal_dark')
    # mount clamp
    box(V, -5, 4, 4, 10, 36, 44, 'chassis')
    # dish arm
    box(V, -1, 0, 2, 3, 36, 44, 'metal')
    # clamp bolts
    tile(V, -3, -3, 11, 11, 37, 37, 'metal', nx=2, nz=2, dx=6, dz=5)
    # cable
    box(V, 3, 3, -11, 6, 3, 3, 'plastic_black'); box(V, 3, 3, 7, 7, 0, 3, 'plastic_black')
    # label
    box(V, -4, 4, 1, 4, 2, 2, 'label'); box(V, -3, 3, 2, 2, 2, 2, 'label_text')
    return V
result = build('AntennaDish', dish(), C, size=F, grain=0)
