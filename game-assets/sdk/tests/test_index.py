from voxel_sdk.rules import lint_spec
from voxel_sdk import index

from conftest import make_spec


def test_long_spec_warns_not_errors(tmp_path):
    p = tmp_path / 'spec.md'
    p.write_text(make_spec(['a: 1 x 1 x 1 cm, a'] * 40))
    errors, warnings = lint_spec(p)
    assert not errors and 'max 40' in warnings[0]


def test_bad_grid_is_error(tmp_path):
    p = tmp_path / 'spec.md'
    p.write_text(make_spec(['a: 1 x 1 x 1 cm, a'], grid='X'))
    assert 'grid' in lint_spec(p)[0][0]


def test_committed_index_is_fresh():
    new = index.render(sorted((index.load(p) for p in index.GAME_ASSETS.glob('t*/*/spec.md')), key=lambda f: f['id']))
    assert index.INDEX.read_text() == new
