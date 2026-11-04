import numpy as np
from voxel_sdk import compile, render

# Grid F: 1 voxel = 0.78125 cm, so 7.8125 cm = 10 voxels, 0.78 cm = 1 voxel.
TWO = ['base: 7.8 x 7.8 x 1.6 cm, a', 'top: 3.9 x 3.9 x 3.9 cm, b']


def cell(grid, x, y, z):
    o = grid.o
    return grid.keys[grid.a[x - o[0], y - o[1], z - o[2]] - 1] if grid.a[x - o[0], y - o[1], z - o[2]] else None


def errs(report):
    return ' | '.join(report.errors)


def test_stacks_part_on_top_centered_and_reports_bounds(element):
    d = element(TWO, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    grid, r = compile(d)
    assert r.ok, r.errors
    assert r.size == (10, 10, 7)               # base 10x10x2, top 5x5x5 on top
    assert r.bounds[0][2] == 0                 # first Part sits on z=0
    assert r.counts == {'a': 200, 'b': 125}
    assert cell(grid, 0, 0, 0) == 'a' and cell(grid, 0, 0, 2) == 'b' and cell(grid, -5, 0, 2) is None


def test_align_flush_and_offset(element):
    d = element(TWO, '[base]\n[top]\non = "base.top"\nalign = "left"\ny = "flush-back"\noffset_z = -0.78\n',
                voxels='10x10x6')
    grid, r = compile(d)
    assert r.ok, r.errors
    lo, hi = r.parts['top']
    blo, bhi = r.parts['base']
    assert lo[0] == blo[0] and hi[1] == bhi[1] and lo[2] == bhi[2]      # overlaps the base by 1 voxel


def test_face_front_places_decal_outside_in_minus_y(element):
    d = element(['base: 7.8 x 7.8 x 7.8 cm, a', 'led: 1 cm, b'],
                '[base]\n[led]\non = "base.front"\n', voxels='10x10x10')
    _, r = compile(d)
    assert r.ok, r.errors
    assert r.parts['led'][1][1] == r.parts['base'][0][1] - 1
    assert r.size == (10, 11, 10)


def test_s_grid_uses_64_per_metre(element):
    d = element(['base: 100 x 100 x 100 cm, a'], '[base]\n', voxels='64x64x64', grid='S')
    _, r = compile(d)
    assert r.size == (64, 64, 64)


def test_missing_dimensions_become_one_voxel_decal(element):
    d = element(['base: 7.8 x 7.8 x 7.8 cm, a', 'dot: 2 x 1 cm, b'],
                '[base]\n[dot]\non = "base.top"\n', voxels='10x10x11')
    _, r = compile(d)
    lo, hi = r.parts['dot']
    assert (hi[0] - lo[0] + 1, hi[1] - lo[1] + 1, hi[2] - lo[2] + 1) == (3, 1, 1)


def test_layout_size_overrides_spec(element):
    d = element(['base: 7.8 x 7.8 x 7.8 cm, a'], '[base]\nsize = [3.9, 3.9, 3.9]\n', voxels='5x5x5')
    _, r = compile(d)
    assert r.ok, r.errors
    assert r.size == (5, 5, 5)


def test_repeat_pitch_converted_and_group_centered(element):
    d = element(['base: 7.8 x 7.8 x 1.6 cm, a', 'slot: 0.78 x 0.78 x 0.78 cm, b'],
                '[base]\n[slot]\non = "base.top"\nrepeat = { x = 3, pitch_x = 1.56 }\n', voxels='10x10x3')
    grid, r = compile(d)
    assert r.ok, r.errors
    assert r.counts['b'] == 3
    lo, hi = r.parts['slot']
    assert hi[0] - lo[0] + 1 == 5          # 3 voxels pitch 2 -> span 5
    y, z = lo[1], lo[2]
    assert cell(grid, lo[0], y, z) == 'b' and cell(grid, lo[0] + 1, y, z) is None and cell(grid, lo[0] + 2, y, z) == 'b'


def test_round_shapes(element):
    parts = ['p: 7.8 x 7.8 x 0.78 cm, a']
    for shape, extra, count in (('disc', '', None), ('circle', '', None), ('ring', 'thickness = 1.5\n', None)):
        d = element(parts, f'[p]\nshape = "{shape}"\n{extra}', voxels='10x10x1')
        grid, r = compile(d)
        assert r.ok, (shape, r.errors)
        n = r.counts['a']
        assert 0 < n < 100
        assert cell(grid, 0, 0, 0) == ('a' if shape == 'disc' else None)    # centre filled only for a disc
    d = element(parts, '[p]\nshape = "disc"\n', voxels='10x10x1')
    disc = compile(d)[1].counts['a']
    d = element(parts, '[p]\nshape = "circle"\n', voxels='10x10x1')
    assert compile(d)[1].counts['a'] < disc


def test_hollow_with_open_face(element):
    d = element(['box: 7.8 x 7.8 x 7.8 cm, a'], '[box]\nshape = "hollow"\nopen = ["top"]\n', voxels='10x10x10')
    grid, r = compile(d)
    assert r.ok, r.errors
    assert cell(grid, 0, 0, 5) is None and cell(grid, 0, 0, 0) == 'a' and cell(grid, 0, 0, 9) is None
    assert cell(grid, -5, 0, 9) == 'a'


def test_raw_block_runs_is_counted_and_validated(element):
    d = element(['base: 7.8 x 7.8 x 1.6 cm, a'],
                "[base]\n[[raw]]\ncode = \"box(V, 0, 1, 0, 1, 2, 3, 'b')\"\n", voxels='10x10x4')
    grid, r = compile(d)
    assert r.ok, r.errors
    assert r.raw_count == 1 and r.counts['b'] == 8
    d = element(['base: 7.8 x 7.8 x 1.6 cm, a'],
                "[base]\n[[raw]]\ncode = \"box(V, 0, 1, 0, 1, 6, 7, 'b')\"\n", voxels='10x10x8')
    assert 'floating' in errs(compile(d)[1])                       # raw goes through the same checks
    d = element(['base: 7.8 x 7.8 x 1.6 cm, a'],
                "[base]\n[[raw]]\ncode = \"box(V, 0, 1, 0, 1, 2, 3, 'nope')\"\n", voxels='10x10x4')
    assert 'unknown palette key "nope"' in errs(compile(d)[1])


def test_unknown_anchor_part_and_face_fail_loudly(element):
    d = element(TWO, '[base]\n[top]\non = "ghost.top"\n')
    assert 'ghost' in errs(compile(d)[1])
    d = element(TWO, '[base]\n[top]\non = "base.roof"\n')
    assert 'roof' in errs(compile(d)[1])
    d = element(TWO, '[base]\n[top]\non = "base.top"\nalign = "middle"\n')
    assert 'middle' in errs(compile(d)[1])
    d = element(TWO, '[base]\n[top]\non = "base.top"\nwobble = 3\n')
    assert 'wobble' in errs(compile(d)[1])


def test_unknown_palette_key_fails(element):
    d = element(['base: 7.8 x 7.8 x 7.8 cm, mystery'], '[base]\n')
    assert 'mystery' in errs(compile(d)[1])


def test_floating_part_detected(element):
    d = element(TWO, '[base]\n[top]\non = "base.top"\ngap = 3\n', voxels='10x10x12')
    assert 'floating' in errs(compile(d)[1]) and 'top' in errs(compile(d)[1])


def test_same_colour_neighbours_flagged(element):
    d = element(['base: 7.8 x 7.8 x 1.6 cm, a', 'top: 3.9 x 3.9 x 3.9 cm, a'], '[base]\n[top]\non = "base.top"\n',
                voxels='10x10x7')
    _, r = compile(d)
    assert r.ok and any('"base"' in w and '"top"' in w and 'a' in w for w in r.warnings)
    _, r = compile(element(TWO, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7'))
    assert not any('share colour' in w for w in r.warnings)


def test_size_outside_tolerance_fails_within_passes(element):
    layout = '[base]\n[top]\non = "base.top"\n'
    assert compile(element(TWO, layout, voxels='10x10x8'))[1].ok         # real height 7, off by 1
    assert 'size z' in errs(compile(element(TWO, layout, voxels='10x10x12'))[1])


def test_spec_layout_name_mismatch(element):
    d = element(TWO, '[base]\n[lid]\non = "base.top"\n')
    assert '"lid"' in errs(compile(d)[1])
    d = element(TWO, '[base]\n')
    r = compile(d)[1]
    assert any('"top"' in w for w in r.warnings)


def test_report_has_preview_and_legend(element):
    d = element(TWO, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    _, r = compile(d)
    assert set(r.slices) and all('A' in s or 'B' in s for s in r.slices.values())
    assert 'front' in ' '.join(r.slices) and r.legend == {'a': 'A', 'b': 'B'}
    assert 'bounds' in str(r)


def test_render_is_self_contained_python_and_deterministic(element):
    d = element(TWO, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    text, _ = render(d)
    compile_ = __builtins__['compile'] if isinstance(__builtins__, dict) else __builtins__.compile
    compile_(text, 'build.py', 'exec')
    assert 'import bpy' in text and 'def ellipse' in text and 'GENERATED' in text
    assert 'box(V, ' in text and "R = build('Thing', V, C, size=FINE_SIZE, grain=0)" in text
    assert render(d)[0] == text


def test_generated_build_py_reproduces_the_compiled_grid(element, monkeypatch):
    """Run the generated script's engine + model with a fake bpy: it must draw exactly what compile() validated."""
    import sys
    from unittest.mock import MagicMock
    monkeypatch.setitem(sys.modules, 'bpy', MagicMock())
    d = element(['base: 7.8 x 7.8 x 7.8 cm, a', 'cup: 3.9 x 3.9 x 3.9 cm, b', 'dots: 0.78 cm, c'],
                '[base]\n[cup]\non = "base.top"\nshape = "ring"\nthickness = 0.78\n'
                '[dots]\non = "base.front"\nrepeat = { x = 3, pitch_x = 1.56 }\n'
                "[[raw]]\ncode = \"box(V, 0, 0, 0, 0, 10, 10, 'b')\"\n", voxels='10x11x15')
    grid, r = compile(d)
    assert r.ok, r.errors
    text, _ = render(d)
    head, model = text.split('# 3. MODEL', 1)
    ns = {}
    exec(head, ns)
    got = {}
    ns['build'] = lambda name, V, C, **kw: got.update(V=V) or {k: None for k in ('object', 'voxels', 'height_m', 'warnings')}
    exec('# 3. MODEL' + model, ns)
    V = got['V']
    counts = {k: int((V.a == i + 1).sum()) for i, k in enumerate(V.keys)}
    assert counts == r.counts
    assert V.bounds() == r.bounds
