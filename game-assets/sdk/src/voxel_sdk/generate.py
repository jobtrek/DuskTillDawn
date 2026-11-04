"""Turn a compiled Model into a self-contained Blender build.py (engine inlined, Alt+P to run)."""
import inspect
import textwrap
from pathlib import Path

from . import shapes
from .core import build_model

HERE = Path(__file__).parent


TILE_DEFAULTS = {'nx': 1, 'ny': 1, 'nz': 1, 'dx': 0, 'dy': 0, 'dz': 0}


def _call(op):
    a = list(op.args)
    pos = ', '.join(str(v) for v in a[:-1]) + f", '{a[-1]}'"
    skip = TILE_DEFAULTS if op.fn == 'tile' else {}
    kw = ''.join(f', {k}={v!r}' for k, v in op.kwargs.items() if skip.get(k, object()) != v)
    return f'{op.fn}(V, {pos}{kw})'


def _single_body(model):
    body = []
    for op in model.ops:
        if op.code is not None:
            body += [f'# {op.part} (raw block)', op.code.strip('\n')]
        else:
            body += [f'# {op.part}', _call(op)]
    return body


def _merge_order(lists):
    """Union of op uids across States, keeping each State's relative order."""
    order = []
    for ops in lists:
        prev = None
        for op in ops:
            if op.uid not in order:
                order.insert(order.index(prev) + 1 if prev is not None else 0, op.uid)
            prev = op.uid
    return order


def _states_body(models):
    """Body of def model(state): ops shared by every State once, colour-only changes as a dict lookup,
    geometry changes and State-only ops in `if state ...` blocks."""
    states = [m.state for m in models]
    by = [{op.uid: op for op in m.ops} for m in models]
    body = []
    for uid in _merge_order([m.ops for m in models]):
        ops = [b.get(uid) for b in by]
        first = next(o for o in ops if o)
        label = f'{first.part} #{uid[1]}' if first.code is None and uid[1] else first.part
        if first.code is not None:
            text = {s: o.code.strip('\n') if o else None for s, o in zip(states, ops)}
            head = f'# {first.part} (raw block)'
        else:
            same_shape = all(o and (o.fn, o.args[:-1], o.kwargs) == (first.fn, first.args[:-1], first.kwargs)
                             for o in ops)
            keys = [o.key for o in ops] if all(ops) else []
            if same_shape and len(set(keys)) > 1:
                lookup = '{' + ', '.join(f"'{s}': '{k}'" for s, k in zip(states, keys)) + '}[state]'
                body += [f'# {label}', _call(first).replace(f"'{first.key}'", lookup, 1)]
                continue
            text = {s: _call(o) if o else None for s, o in zip(states, ops)}
            head = f'# {label}'
        groups = {}
        for s, t in text.items():
            groups.setdefault(t, []).append(s)
        body.append(head)
        if len(groups) == 1 and None not in groups:
            body.append(next(iter(groups)))
            continue
        for t, ss in groups.items():
            if t is None:
                continue
            cond = f"state == '{ss[0]}'" if len(ss) == 1 else f'state in {tuple(ss)!r}'
            body += [f'if {cond}:', textwrap.indent(t, '    ')]
    return body


def render(element_dir):
    """Returns (build.py text, Report of the base State). Raises ValueError if any State has errors."""
    d = Path(element_dir)
    model = build_model(d)
    models = [model] + [build_model(d, state=s) for s in model.spec.states[1:]]
    errors = list(dict.fromkeys(e for m in models for e in m.report.errors))
    if errors:
        raise ValueError('cannot build, fix errors first:\n' + '\n'.join(errors))
    spec = model.spec
    spec_c = '\n'.join('#   ' + l if l else '#' for l in spec.text.splitlines())
    palette = 'PALETTE = ' + repr(model.palette)
    fns = {op.fn for m in models for op in m.ops} | {f for m in models for op in m.ops if op.code
                                                      for f in ('shell', 'ellipse') if f in op.code}
    extras = '\n\n'.join(inspect.getsource(f).rstrip() for f in (shapes.shell, shapes.ellipse) if f.__name__ in fns)
    extras = f'\n\n# SDK extra shapes\n{extras}' if extras else ''
    used = {k for m in models for k in m.grid.keys}
    size = 'FINE_SIZE' if spec.grid == 'F' else 'VOXEL_SIZE'
    metal = ", metallic=('metal',)" if 'metal' in used else ''
    bar = '# ' + '=' * 70
    kw = f'size={size}, grain={spec.grain:g}{metal}'
    if spec.states:
        inner = textwrap.indent('\n'.join(['V = Grid()', *_states_body(models), 'return V, C']), '    ')
        model_code = (f"C = {{**PALETTE}}\n\n\ndef model(state):\n{inner}\n\n\n"
                      f"R = build_states('{spec.name}', model, {spec.states!r}, {kw})\n"
                      "result = {s: {k: r[k] for k in ('object', 'voxels', 'height_m', 'warnings')} for s, r in R.items()}")
    else:
        model_code = (f"C = {{**PALETTE}}\nV = Grid()\n" + '\n'.join(_single_body(model)) +
                      f"\nR = build('{spec.name}', V, C, {kw})\n"
                      "result = {k: R[k] for k in ('object', 'voxels', 'height_m', 'warnings')}")
    out = f"""# {spec.name} ({spec.id}) -- fully self-contained Blender script.
# GENERATED by voxel-sdk from spec.md + layout.toml. Do not edit: change layout.toml and rebuild.
# Usage: Blender > Scripting workspace > Open this file > Run Script (Alt+P).
# Needs Blender 4.x/5.x (bundled numpy). No other files needed.
#
# ---- spec.md (design intent) ----
{spec_c}

{bar}
# 1. VOXEL ENGINE (grid, shapes, greedy meshing, materials, export)
{bar}
{(HERE / 'engine_blender.py').read_text().rstrip()}{extras}


{bar}
# 2. COLOUR PALETTE (linear RGB 0-1)
{bar}
{palette}


{bar}
# 3. MODEL: {spec.name} (generated from layout.toml)
{bar}
{model_code}
"""
    compile(out, 'build.py', 'exec')
    return out, model.report


def write_build(element_dir):
    text, report = render(element_dir)
    (Path(element_dir) / 'build.py').write_text(text)
    return report
