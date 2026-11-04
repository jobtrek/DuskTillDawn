import textwrap
import pytest

PALETTE = ("PALETTE = {'a': (0.1, 0.1, 0.1), 'b': (0.5, 0.5, 0.5), 'c': (0.9, 0.9, 0.9),\n"
           "           'metal': (0.5, 0.5, 0.5)}\n")


def make_spec(parts, voxels='10x10x10', grid='F', name='Thing', states=None):
    lines = '\n'.join(f'- [2] {p}' for p in parts)
    st = ''.join(f'  {s}: note\n' for s in states) if states else ''
    st = f'states:\n{st}' if st else ''
    return (f'---\nname: {name}\nid: t9-01\ntier: T9\nsize_cm: 1x1x1\nvoxels: {voxels}\ngrid: {grid}\ngrain: 0\n{st}---\n\n'
            f'# {name}\n\n## Parts (tier; cm size + colour key)\n\n{lines}\n\n## Notes\n\n- none\n')


@pytest.fixture
def element(tmp_path):
    """element(parts=[...spec lines], layout='toml', voxels='WxDxH', grid='F', states=[...]) -> Element dir."""
    (tmp_path / 'palette.py').write_text(PALETTE)

    def make(parts, layout, voxels='10x10x10', grid='F', states=None):
        d = tmp_path / 'el'
        d.mkdir(exist_ok=True)
        (d / 'spec.md').write_text(make_spec(parts, voxels, grid, states=states))
        (d / 'layout.toml').write_text(textwrap.dedent(layout))
        return d
    return make
