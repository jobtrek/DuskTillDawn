"""voxel-sdk check <dir> [--state <State>] | build <dir>"""
import sys

import numpy as np

from .core import build_model
from .generate import write_build

USAGE = 'usage: voxel-sdk check <element-dir> [--state <State>] | voxel-sdk build <element-dir>'


def _voxels(grid):
    return {tuple(int(v) for v in p + grid.o): grid.keys[grid.a[tuple(p)] - 1] for p in np.argwhere(grid.a)}


def _diff_line(state, base, other):
    a, b = _voxels(base.grid), _voxels(other.grid)
    changed = sum(a.get(p) != b.get(p) for p in a.keys() | b.keys())
    keys = dict.fromkeys([*base.report.counts, *other.report.counts])
    deltas = [(k, other.report.counts.get(k, 0) - base.report.counts.get(k, 0)) for k in keys]
    deltas = ', '.join(f'{k} {d:+d}' for k, d in deltas if d)
    return f'{state}: {changed} voxels changed' + (f' ({deltas})' if deltas else '')


def _strip(msg):
    return msg.split('] ', 1)[1] if msg.startswith('[') else msg


def check(d, state=None):
    base = build_model(d, state=state)
    print(base.report)
    ok = base.report.ok
    if state is not None or not base.spec.states:
        return 0 if ok else 1
    seen = {_strip(m) for m in base.report.errors + base.report.warnings}
    print('\n-- other States (vs base ' + base.state + '; full preview: --state <State>) --')
    for s in base.spec.states[1:]:
        m = build_model(d, state=s)
        ok &= m.report.ok
        print(_diff_line(s, base, m) if m.report.bounds and base.report.bounds else f'{s}: not compiled')
        for e in m.report.errors:
            print(f'  ERROR: {e}')
        for w in m.report.warnings:
            if _strip(w) not in seen:
                print(f'  warning: {w}')
    print('ALL STATES OK' if ok else 'FAILED')
    return 0 if ok else 1


def main(argv=None):
    argv = sys.argv[1:] if argv is None else argv
    state = None
    argv = [x for a in argv for x in (a.split('=', 1) if a.startswith('--state=') else [a])]
    if '--state' in argv:
        i = argv.index('--state')
        if i + 1 >= len(argv):
            print(USAGE)
            return 2
        state = argv[i + 1]
        argv = argv[:i] + argv[i + 2:]
    if len(argv) != 2 or argv[0] not in ('check', 'build') or (state and argv[0] == 'build'):
        print(USAGE)
        return 2
    cmd, d = argv
    if cmd == 'check':
        return check(d, state)
    try:
        report = write_build(d)
    except ValueError as e:
        print(e)
        return 1
    print(report)
    print(f'wrote {d}/build.py')
    return 0
