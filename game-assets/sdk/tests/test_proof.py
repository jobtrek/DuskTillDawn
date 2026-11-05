"""Proof: the migrated Layouts must match the hand-written builds they replace.
REFERENCE was measured by running the original hand-written build.py model sections (git 5ab9393)
on the same pure-numpy shapes: (bounds size x,y,z, per-colour voxel counts), per State where the Element has States."""
from pathlib import Path

import pytest
from voxel_sdk import compile, render

ASSETS = Path(__file__).resolve().parents[2]

REFERENCE = {
    't2-small/t2-17-Monitor': ((69, 23, 46), {'chassis': 3615, 'chassis_dark': 196, 'plastic_black': 13238, 'seam': 274,
                                              'screen': 1890, 'led_green': 2, 'trim': 8, 'metal_dark': 87, 'metal': 20}),
    # Mug reference is the Full state; the Mug Layout has no [states] overrides yet, so only Full is compared.
    't1-tiny/t1-06-Mug': ((14, 10, 12), {'chassis_dark': 64, 'plastic_white': 492, 'trim': 35, 'warning_red': 20,
                                         'wood_dark': 512}),
    # Server1U reference is the On state.
    't2-small/t2-19-Server1U': ((58, 77, 6), {'chassis': 23892, 'chassis_dark': 532, 'metal': 140, 'metal_dark': 76,
                                              'seam': 360, 'led_green': 8, 'trim': 4, 'label': 80}),
}

_SRV = {'chassis': 23892, 'chassis_dark': 532, 'metal': 140, 'metal_dark': 76, 'trim': 4, 'label': 80}
STATE_REFERENCE = {
    't2-small/t2-19-Server1U': {
        'On': ((58, 77, 6), {**_SRV, 'seam': 360, 'led_green': 8}),
        'Hot': ((58, 77, 6), {**_SRV, 'seam': 32, 'led_green': 4, 'led_orange': 332}),
        'Off': ((58, 77, 6), {**_SRV, 'seam': 360, 'led_off': 8}),
        'Failed': ((58, 77, 6), {**_SRV, 'seam': 360, 'led_off': 4, 'led_red': 4}),
    },
    't2-small/t2-24-Laptop': {
        s: ((42, 28, 3), {'rubber': 24, 'metal': 1152, 'chassis': 1146, 'metal_dark': 24, 'trim': 16,
                          'plastic_black': 4, **extra})
        for s, extra in {'On': {'seam': 6, 'led_green': 4}, 'Hot': {'led_orange': 10},
                         'Failed': {'seam': 6, 'led_red': 4}, 'Off': {'seam': 6, 'led_off': 4}}.items()
    },
    't2-small/t2-26-PDU': {
        s: ((56, 13, 6), {'chassis': 3266, 'metal': 156, 'plastic_black': 92, 'label': 50, 'rubber': 32, **extra})
        for s, extra in {'On': {'plastic_white': 4, 'led_green': 4}, 'Overload': {'plastic_white': 4, 'led_orange': 4},
                         'Tripped': {'warning_red': 4, 'led_red': 4}, 'Off': {'trim': 4, 'led_off': 4}}.items()
    },
}
# The old Server1U build never ejected drive bay 2 although its Spec asks for it (Failed State).
EJECTED = {('t2-small/t2-19-Server1U', 'Failed'): {'size_y': 2, 'metal_dark': 48}}
# Keys whose count differs between States: the State-specific detail must match the old build closely.
TIGHT = 2


@pytest.mark.parametrize('element', REFERENCE)
def test_layout_matches_existing_build_within_one_voxel(element):
    size, counts = REFERENCE[element]
    _, r = compile(ASSETS / element)
    assert r.ok, r.errors
    assert all(abs(a - b) <= 1 for a, b in zip(r.size, size)), (r.size, size)
    assert set(r.counts) <= set(counts)                      # no colour the old build did not use
    # the old Mug fills the cup with coffee volume; the Spec's "coffee surface" is one layer, so track the ceramic
    dominant = 'plastic_white' if 'Mug' in element else max(counts, key=counts.get)
    assert abs(r.counts[dominant] - counts[dominant]) / counts[dominant] < 0.05


@pytest.mark.parametrize('element, state', [(e, s) for e, ss in STATE_REFERENCE.items() for s in ss])
def test_every_state_matches_existing_build(element, state):
    size, counts = STATE_REFERENCE[element][state]
    _, r = compile(ASSETS / element, state=state)
    assert r.ok, r.errors
    ej = EJECTED.get((element, state), {})
    got = list(r.size)
    got[1] -= ej.get('size_y', 0)
    assert all(abs(a - b) <= 1 for a, b in zip(got, size)), (r.size, size)
    assert set(r.counts) == set(counts), (r.counts, counts)
    dominant = max(counts, key=counts.get)
    assert abs(r.counts[dominant] - counts[dominant]) / counts[dominant] < 0.05
    varying = {k for ss in STATE_REFERENCE[element].values() for k in counts if ss[1].get(k) != counts[k]}
    for k in varying:
        assert abs(r.counts[k] - ej.get(k, 0) - counts[k]) <= TIGHT, (k, r.counts[k], counts[k])


# build.py is generated and git-ignored; check the thin form stays small and the standalone form is complete.
@pytest.mark.parametrize('element', ['t2-small/t2-17-Monitor', *STATE_REFERENCE])
def test_thin_build_is_short_and_standalone_is_complete(element):
    thin, _ = render(ASSETS / element)
    full, _ = render(ASSETS / element, standalone=True)
    assert len(thin.splitlines()) < 120 < len(full.splitlines())
    assert 'engine_blender.py' in thin and 'def build(' not in thin and 'def build(' in full
