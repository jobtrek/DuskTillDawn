"""Build kwargs the hand-written builds pass: metallic tuple, collider, per-Element extra colours."""
from voxel_sdk import compile, render

ONE = ['base: 7.8 x 7.8 x 1.6 cm, a', 'top: 3.9 x 3.9 x 3.9 cm, metal']


def model_line(text):
    return next(l for l in text.splitlines() if l.startswith('R = build'))


def test_build_table_sets_metallic_in_order(element):
    d = element(ONE, '[build]\nmetallic = ["metal", "a"]\n[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    _, r = compile(d)
    assert r.ok, r.errors
    assert "metallic=('metal', 'a')" in model_line(render(d)[0])


def test_build_table_metallic_empty_list_emits_no_metallic(element):
    d = element(ONE, '[build]\nmetallic = []\n[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    assert 'metallic' not in model_line(render(d)[0])


def test_default_metallic_unchanged_without_build_table(element):
    d = element(ONE, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    assert "metallic=('metal',)" in model_line(render(d)[0])


def test_spec_collider_true_is_emitted(element):
    d = element(ONE, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    spec = d / 'spec.md'
    spec.write_text(spec.read_text().replace('grain: 0\n', 'grain: 0\ncollider: true\n', 1))
    assert 'collider=True' in model_line(render(d)[0])


def test_no_collider_kwarg_when_spec_has_none(element):
    d = element(ONE, '[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    assert 'collider' not in model_line(render(d)[0])


def test_build_table_collider_false_is_emitted(element):
    d = element(ONE, '[build]\ncollider = false\n[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    assert 'collider=False' in model_line(render(d)[0])


def test_unknown_build_key_is_an_error(element):
    d = element(ONE, '[build]\nshiny = true\n[base]\n[top]\non = "base.top"\n', voxels='10x10x7')
    _, r = compile(d)
    assert any('[build]' in e and 'shiny' in e for e in r.errors), r.errors


def test_palette_table_adds_element_colours(element):
    d = element(['base: 7.8 x 7.8 x 1.6 cm, cable'], '[palette]\ncable = [0.05, 0.05, 0.07]\n[base]\n',
                voxels='10x10x2')
    _, r = compile(d)
    assert r.ok, r.errors
    text, _ = render(d)
    assert "'cable': (0.05, 0.05, 0.07)" in text
