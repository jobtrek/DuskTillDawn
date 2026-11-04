"""3-model proof: the migrated Layouts must match the hand-written builds they replace.
REFERENCE was measured by running the original hand-written build.py model sections (git 5ab9393)
on the same pure-numpy shapes: (bounds size x,y,z, per-colour voxel counts)."""
from pathlib import Path

import pytest
from voxel_sdk import compile, render

ASSETS = Path(__file__).resolve().parents[2]

REFERENCE = {
    't2-small/t2-17-Monitor': ((69, 23, 46), {'chassis': 3615, 'chassis_dark': 196, 'plastic_black': 13238, 'seam': 274,
                                              'screen': 1890, 'led_green': 2, 'trim': 8, 'metal_dark': 87, 'metal': 20}),
    # Mug reference is the Full state; the Layout builds the base state only (States stay hand-authored in v1).
    't1-tiny/t1-06-Mug': ((14, 10, 12), {'chassis_dark': 64, 'plastic_white': 492, 'trim': 35, 'warning_red': 20,
                                         'wood_dark': 512}),
    # Server1U reference is the On state.
    't2-small/t2-19-Server1U': ((58, 77, 6), {'chassis': 23892, 'chassis_dark': 532, 'metal': 140, 'metal_dark': 76,
                                              'seam': 360, 'led_green': 8, 'trim': 4, 'label': 80}),
}


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


# build.py is generated and git-ignored; check the thin form stays small and the standalone form is complete.
@pytest.mark.parametrize('element', ['t2-small/t2-17-Monitor'])
def test_thin_build_is_short_and_standalone_is_complete(element):
    thin, _ = render(ASSETS / element)
    full, _ = render(ASSETS / element, standalone=True)
    assert len(thin.splitlines()) < 120 < len(full.splitlines())
    assert 'engine_blender.py' in thin and 'def build(' not in thin and 'def build(' in full
