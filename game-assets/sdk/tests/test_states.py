"""States (spec v2): overrides on the base Layout, checked at the compile(element_dir, state=...) seam."""
import numpy as np
import pytest
from voxel_sdk import compile, render
from voxel_sdk import shapes

# Grid F: 0.78 cm = 1 voxel. base 10x10x2, led 2x1x2 sunk into the front, pins 3 copies on top.
PARTS = ['base: 7.8 x 7.8 x 1.6 cm, a', 'led: 1.6 x 0.78 x 1.56 cm, b', 'pins: 0.78 x 0.78 x 0.78 cm, c']
BASE = '''
[base]
[led]
on = "base.front"
gap = -0.78
[pins]
on = "base.top"
repeat = { x = 3, pitch_x = 2.34 }
'''
STATES = ['On', 'Hot', 'Off']


def voxels(grid):
    return {tuple(int(v) for v in p + grid.o): grid.keys[grid.a[tuple(p)] - 1] for p in np.argwhere(grid.a)}


def make(element, extra='', parts=PARTS, layout=BASE, states=STATES):
    return element(parts, layout + extra, voxels='10x10x3', states=states)


def test_base_state_is_first_spec_state_and_ignores_overrides(element):
    d = make(element, '[states.Hot]\nled = { key = "metal" }\n')
    _, r = compile(d)
    assert r.ok, r.errors
    assert r.state == 'On'
    assert r.counts == {'a': 196, 'b': 4, 'c': 3}


def test_colour_override_swaps_key_only_in_that_state(element):
    d = make(element, '[states.Hot]\nled = { key = "metal" }\n')
    base, _ = compile(d)
    hot, r = compile(d, state='Hot')
    assert r.ok, r.errors
    assert r.counts == {'a': 196, 'metal': 4, 'c': 3}
    changed = {p for p in voxels(base).keys() | voxels(hot).keys() if voxels(base).get(p) != voxels(hot).get(p)}
    assert len(changed) == 4 and all(voxels(hot)[p] == 'metal' for p in changed)


def test_override_moves_part_and_parts_anchored_on_it_follow(element):
    parts = PARTS + ['dot: 0.78 x 0.78 x 0.78 cm, metal']
    layout = BASE + '[dot]\non = "led.front"\n'
    d = make(element, '[states.Hot]\nled = { offset_x = 1.56 }\n', parts=parts, layout=layout)
    _, on = compile(d)
    _, hot = compile(d, state='Hot')
    assert hot.ok, hot.errors
    assert hot.parts['led'][0][0] == on.parts['led'][0][0] + 2
    assert hot.parts['dot'][0][0] == on.parts['dot'][0][0] + 2


def test_instance_override_resizes_one_copy_away_from_the_host_face(element):
    d = make(element, '[states.Hot]\n"pins#2" = { size = [0.78, 0.78, 2.34] }\n')
    on, _ = compile(d)
    hot, r = compile(d, state='Hot')
    assert r.ok, r.errors
    pins = lambda g: sorted(p for p, k in voxels(g).items() if k == 'c')
    assert len(pins(on)) == 3 and len(pins(hot)) == 5          # copy 2 grows from 1 to 3 voxels tall
    x2 = sorted({p[0] for p in pins(on)})[1]
    assert sorted(p[2] for p in pins(hot) if p[0] == x2) == [2, 3, 4]     # grows up, away from base.top
    assert any('size z' in w for w in r.warnings)               # non-base State past the Spec size: warning only


def test_instance_override_on_front_face_grows_forward(element):
    parts = PARTS + ['bays: 0.78 x 0.78 x 0.78 cm, metal']
    layout = BASE + '[bays]\non = "base.front"\ngap = -0.78\nalign = "left"\nrepeat = { x = 2, pitch_x = 7 }\n'
    d = make(element, '[states.Hot]\n"bays#1" = { size = [0.78, 2.34, 0.78] }\n', parts=parts, layout=layout)
    on, _ = compile(d)
    hot, r = compile(d, state='Hot')
    assert r.ok, r.errors
    front = min(p[1] for p in voxels(on))
    bay1 = [p for p, k in voxels(hot).items() if k == 'metal' and p[0] == min(q[0] for q in voxels(hot))]
    assert sorted(p[1] for p in bay1) == [front - 2, front - 1, front]


def test_omit_and_state_only_parts(element):
    layout = BASE.replace('[pins]\n', '[pins]\nstates = ["Hot"]\n')
    d = make(element, '[states.Off]\nled = { omit = true }\n', layout=layout)
    _, on = compile(d)
    _, hot = compile(d, state='Hot')
    _, off = compile(d, state='Off')
    assert on.ok and hot.ok and off.ok, (on.errors, hot.errors, off.errors)
    assert 'c' not in on.counts and hot.counts['c'] == 3
    assert 'b' not in off.counts and off.counts['a'] == 200


def test_state_raw_block_runs_only_in_that_state(element):
    d = make(element, '[[states.Hot.raw]]\ncode = "box(V, 0, 0, 0, 0, 2, 2, \'metal\')"\n')
    _, on = compile(d)
    _, hot = compile(d, state='Hot')
    assert 'metal' not in on.counts and hot.counts['metal'] == 1
    assert hot.raw_count == 1


@pytest.mark.parametrize('extra, state, message', [
    ('[states.Cold]\nled = { key = "metal" }\n', None, 'unknown State "Cold"'),
    ('[states.Hot]\nlamp = { key = "metal" }\n', None, 'unknown Part "lamp"'),
    ('[states.Hot]\nled = { colour = "metal" }\n', None, "unknown override key(s) ['colour']"),
    ('[states.Hot]\n"pins#x" = { key = "metal" }\n', None, 'copy number must be a whole number'),
    ('[states.Hot]\n"pins#4" = { key = "metal" }\n', 'Hot', '"pins#4" out of range, the Part has 3 copies'),
    ('[states.Hot]\nled = { key = "nope" }\n', 'Hot', 'unknown palette key "nope"'),
    ('[states.Hot]\nled = { offset_y = -2.34 }\n', 'Hot', 'floating Part(s)'),
])
def test_state_errors_are_loud_and_prefixed(element, extra, state, message):
    d = make(element, extra)
    _, r = compile(d, state=state)
    assert not r.ok
    assert any(message in e for e in r.errors), r.errors
    assert all(e.startswith(f'[{state or "On"}] ') for e in r.errors), r.errors


def test_state_only_part_naming_unknown_state_is_an_error(element):
    d = make(element, layout=BASE.replace('[pins]\n', '[pins]\nstates = ["Warm"]\n'))
    _, r = compile(d)
    assert any("states = ['Warm'] not in the Spec states" in e for e in r.errors), r.errors


def test_states_table_needs_spec_states(element):
    d = make(element, '[states.Hot]\nled = { key = "metal" }\n', states=None)
    _, r = compile(d)
    assert any('Layout has [states] but the Spec has no states:' in e for e in r.errors), r.errors


def test_unknown_state_argument(element):
    _, r = compile(make(element), state='Warm')
    assert any('unknown State "Warm"' in e for e in r.errors)


def _run_generated(text):
    """Execute the generated MODEL section on the pure-numpy shapes; returns {state: voxels}."""
    ns = {n: getattr(shapes, n) for n in dir(shapes) if not n.startswith('_')}
    ns.update(PALETTE={}, FINE_SIZE=0.0078125, VOXEL_SIZE=0.015625,
              build_states=lambda name, model, states, **kw: {s: model(s) for s in states})
    exec(text[text.index('# 3. MODEL'):].split('\nresult =')[0], ns)
    return {s: voxels(V) for s, (V, _) in ns['R'].items()}


def test_generated_build_has_model_function_and_matches_every_state(element):
    extra = ('[states.Hot]\nled = { key = "metal" }\n"pins#2" = { offset_x = 0.78 }\n'
             '[states.Off]\nled = { omit = true }\n')
    d = make(element, extra)
    text, _ = render(d)
    assert 'def model(state):' in text and "R = build_states('Thing', model, ['On', 'Hot', 'Off']" in text
    assert "{'On': 'b', 'Hot': 'metal', 'Off'" not in text                  # led is omitted in Off: no dict lookup
    assert "if state in ('On', 'Off'):" in text and "if state == 'Hot':" in text     # pins#2 moves in Hot only
    got = _run_generated(text)
    for s in STATES:
        assert got[s] == voxels(compile(d, state=s)[0]), s


def test_generated_colour_only_change_is_a_dict_lookup(element):
    d = make(element, '[states.Hot]\nled = { key = "metal" }\n')
    text, _ = render(d)
    assert "{'On': 'b', 'Hot': 'metal', 'Off': 'b'}[state]" in text
    assert _run_generated(text)['Hot'] == voxels(compile(d, state='Hot')[0])


def test_build_refuses_when_any_state_has_errors(element):
    d = make(element, '[states.Off]\nled = { key = "nope" }\n')
    with pytest.raises(ValueError, match=r'\[Off\] unknown palette key "nope"'):
        render(d)


def test_state_names_parse_flat_list_and_report_bad_names(element, tmp_path):
    from voxel_sdk.spec import load_spec
    d = make(element, states=['On', 'Half-Full'])
    _, r = compile(d)
    assert any("State name(s) ['Half-Full']" in e for e in r.errors), r.errors
    p = tmp_path / 'flat.md'
    p.write_text(make_spec_text().replace('grain: 0\n', 'grain: 0\nstates: [On, Off]\n'))
    assert load_spec(p).states == ['On', 'Off']


def make_spec_text():
    from conftest import make_spec
    return make_spec(PARTS)


def test_override_on_part_absent_in_state_is_an_error(element):
    layout = BASE.replace('[pins]\n', '[pins]\nstates = ["Hot"]\n')
    d = make(element, '[states.Off]\n"pins#2" = { key = "metal" }\n', layout=layout)
    _, r = compile(d, state='Off')
    assert any('Part "pins" is not placed in this State' in e for e in r.errors), r.errors


def test_state_with_no_parts_is_an_error(element):
    d = make(element, '[states.Off]\nbase = { omit = true }\nled = { omit = true }\npins = { omit = true }\n')
    _, r = compile(d, state='Off')
    assert any('no Parts placed in this State' in e for e in r.errors), r.errors
