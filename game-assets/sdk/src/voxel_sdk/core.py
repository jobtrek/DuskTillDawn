"""compile(element_dir): Spec + Layout -> voxel grid + report. No Blender needed."""
import math
import tomllib
from dataclasses import dataclass, field
from pathlib import Path

import numpy as np

from . import shapes
from .spec import load_spec

FACES = {'top': (2, 1), 'bottom': (2, -1), 'front': (1, -1), 'back': (1, 1), 'left': (0, -1), 'right': (0, 1)}
ALIGN = {'center-x': (0, 'c'), 'left': (0, 'lo'), 'right': (0, 'hi'),
         'center-y': (1, 'c'), 'center-z': (2, 'c'), 'bottom': (2, 'lo'), 'top': (2, 'hi')}
YFLUSH = {'flush-front': 'lo', 'flush-back': 'hi'}
PART_KEYS = {'on', 'align', 'y', 'gap', 'offset_x', 'offset_y', 'offset_z', 'repeat', 'size', 'shape', 'thickness', 'open'}
SHAPES = {'box', 'hollow', 'disc', 'circle', 'ring'}
TOLERANCE = 1


@dataclass
class Report:
    errors: list = field(default_factory=list)
    warnings: list = field(default_factory=list)
    bounds: tuple | None = None          # (lo, hi) inclusive voxel coords
    size: tuple | None = None            # voxels per axis
    counts: dict = field(default_factory=dict)
    raw_count: int = 0
    parts: dict = field(default_factory=dict)   # name -> (lo, hi) placement box in voxels
    slices: dict = field(default_factory=dict)
    legend: dict = field(default_factory=dict)

    @property
    def ok(self):
        return not self.errors

    def __str__(self):
        out = [f"bounds {self.bounds}  size {self.size}  raw blocks {self.raw_count}",
               'colours: ' + ', '.join(f'{k}={v}' for k, v in self.counts.items())]
        out += [f'ERROR: {e}' for e in self.errors] + [f'warning: {w}' for w in self.warnings]
        for name, art in self.slices.items():
            out += ['', f'-- {name} --', art]
        out += ['', 'legend: ' + '  '.join(f'{c}={k}' for k, c in self.legend.items())]
        out.append('OK' if self.ok else f'FAILED ({len(self.errors)} errors)')
        return '\n'.join(out)


@dataclass
class Op:
    part: str            # Part name, or 'raw#n'
    fn: str              # name in shapes
    args: tuple          # positional args after V
    kwargs: dict
    key: str | None = None
    code: str | None = None     # raw blocks only


@dataclass
class Model:
    spec: object
    grid: shapes.Grid
    report: Report
    ops: list
    palette: dict


def _round(x):
    return math.floor(x + 0.5)


def _find_palette(element_dir):
    for p in [element_dir, *element_dir.resolve().parents]:
        f = p / 'palette.py'
        if f.exists():
            ns = {}
            exec(f.read_text(), ns)
            return ns['PALETTE'], f
    raise FileNotFoundError(f'palette.py not found above {element_dir}')


def _resolve(spec, layout, report):
    """Place every Part. Returns (boxes {name: (lo, hi)}, ops). Errors go to the report."""
    cmpv = spec.cm_per_voxel
    vox = lambda cm: max(1, _round(cm / cmpv))
    sv = lambda cm: _round(cm / cmpv)       # signed, may be 0
    boxes, ops, order = {}, [], []
    for name, cfg in layout.items():
        if name == 'raw':
            continue
        where = f'Part "{name}"'
        if not isinstance(cfg, dict):
            report.errors.append(f'{where}: Layout entry must be a table')
            continue
        if name not in spec.parts:
            report.errors.append(f'{where} is in the Layout but not in the Spec Parts list')
            continue
        bad = set(cfg) - PART_KEYS
        if bad:
            report.errors.append(f'{where}: unknown Layout key(s) {sorted(bad)}')
            continue
        sp = spec.parts[name]
        cm = cfg.get('size') or sp.size_cm
        if not cm:
            report.errors.append(f'{where}: no size in Spec and no size= in Layout')
            continue
        s = [vox(c) for c in cm] + [1] * (3 - len(cm))
        s = s[:3]
        shape = cfg.get('shape', 'box')
        if shape not in SHAPES:
            report.errors.append(f'{where}: unknown shape "{shape}" (use {sorted(SHAPES)})')
            continue
        rep = cfg.get('repeat', {})
        n, pitch = [1, 1, 1], [0, 0, 0]
        for i, ax in enumerate('xyz'):
            n[i] = int(rep.get(ax, 1))
            if n[i] > 1:
                if f'pitch_{ax}' not in rep:
                    report.errors.append(f'{where}: repeat {ax}={n[i]} needs pitch_{ax}')
                    break
                pitch[i] = max(1, _round(rep[f'pitch_{ax}'] / cmpv))
        else:
            ext = [s[i] + (n[i] - 1) * pitch[i] for i in range(3)]
            lo = _place(name, cfg, ext, boxes, order, sv, report, where)
            if lo is None:
                continue
            hi = [lo[i] + ext[i] - 1 for i in range(3)]
            boxes[name] = (lo, hi)
            order.append(name)
            ops += _ops_for(name, sp.color, shape, cfg, lo, s, n, pitch, vox, report, where)
    return boxes, ops


def _place(name, cfg, ext, boxes, order, sv, report, where):
    lo = [0, 0, 0]
    on = cfg.get('on')
    aligns = cfg.get('align', [])
    aligns = [aligns] if isinstance(aligns, str) else list(aligns)
    face_axis = None
    if on is None:
        if order:
            report.errors.append(f'{where}: needs on="<part>.<face>" (only the first Part may omit it)')
            return None
        lo = [-(ext[0] // 2), -(ext[1] // 2), 0]
        if aligns or 'y' in cfg or 'gap' in cfg:
            report.errors.append(f'{where}: align/y/gap need on=; the first Part is placed at the origin (use offset_*)')
            return None
    else:
        host, _, face = str(on).rpartition('.')
        if host not in boxes:
            report.errors.append(f'{where}: on="{on}" unknown anchor, no Part "{host}" placed before it')
            return None
        if face not in FACES:
            report.errors.append(f'{where}: on="{on}" unknown face "{face}" (use {sorted(FACES)})')
            return None
        hlo, hhi = boxes[host]
        face_axis, sign = FACES[face]
        gap = sv(cfg.get('gap', 0))
        for ax in range(3):
            lo[ax] = hlo[ax] + (hhi[ax] - hlo[ax] + 1 - ext[ax]) // 2     # centred by default
        lo[face_axis] = (hhi[face_axis] + 1 + gap) if sign > 0 else (hlo[face_axis] - gap - ext[face_axis])
        todo = []
        for a in aligns:
            if a not in ALIGN:
                report.errors.append(f'{where}: unknown align "{a}" (use {sorted(ALIGN)})')
                return None
            todo.append(ALIGN[a])
        if 'y' in cfg:
            if cfg['y'] not in YFLUSH:
                report.errors.append(f'{where}: unknown y "{cfg["y"]}" (use {sorted(YFLUSH)})')
                return None
            todo.append((1, YFLUSH[cfg['y']]))
        for ax, how in todo:
            if ax == face_axis:
                report.errors.append(f'{where}: align on the {"xyz"[ax]} axis conflicts with on="{on}"')
                return None
            lo[ax] = {'c': lo[ax], 'lo': hlo[ax], 'hi': hhi[ax] - ext[ax] + 1}[how]
    for ax, k in enumerate(('offset_x', 'offset_y', 'offset_z')):
        lo[ax] += sv(cfg.get(k, 0))
    return lo


def _ops_for(name, color, shape, cfg, lo, s, n, pitch, vox, report, where):
    """Engine calls for one Part. All coordinates are final voxel coordinates (before the global shift)."""
    t = vox(cfg['thickness']) if 'thickness' in cfg else 1
    opn = tuple(cfg.get('open', ()))
    for f in opn:
        if f not in FACES:
            report.errors.append(f'{where}: open face "{f}" unknown')
            return []
    hi = [lo[i] + s[i] - 1 for i in range(3)]
    if shape == 'box' and max(n) > 1:
        return [Op(name, 'tile', (lo[0], hi[0], lo[1], hi[1], lo[2], hi[2], color),
                   {'nx': n[0], 'ny': n[1], 'nz': n[2], 'dx': pitch[0], 'dy': pitch[1], 'dz': pitch[2]}, color)]
    out = []
    for i in range(n[0]):
        for j in range(n[1]):
            for k in range(n[2]):
                a = [lo[0] + i * pitch[0], lo[1] + j * pitch[1], lo[2] + k * pitch[2]]
                b = [a[0] + s[0] - 1, a[1] + s[1] - 1, a[2] + s[2] - 1]
                c = (a[0], b[0], a[1], b[1], a[2], b[2])
                if shape == 'box':
                    out.append(Op(name, 'box', (*c, color), {}, color))
                elif shape == 'hollow' and opn:
                    out.append(Op(name, 'shell', (*c, color), {'t': t, 'open': opn}, color))
                elif shape == 'hollow':
                    out.append(Op(name, 'hollow', (*c, color), {'t': t} if t != 1 else {}, color))
                else:
                    wall = {'disc': 0, 'circle': 1, 'ring': t}[shape]
                    out.append(Op(name, 'ellipse', (*c, color), {'t': wall} if wall else {}, color))
    return out


def _shift(ops, boxes):
    lo = [min(b[0][i] for b in boxes.values()) for i in range(3)]
    hi = [max(b[1][i] for b in boxes.values()) for i in range(3)]
    sh = (-lo[0] - (hi[0] - lo[0] + 1) // 2, 0, -lo[2])
    for op in ops:
        a = list(op.args)
        for i in range(6):
            a[i] += sh[i // 2]
        op.args = tuple(a)
    return {k: (tuple(l + sh[i] for i, l in enumerate(b[0])), tuple(h + sh[i] for i, h in enumerate(b[1])))
            for k, b in boxes.items()}


def _raw_namespace():
    ns = {n: getattr(shapes, n) for n in ('Grid', 'box', 'hollow', 'tile', 'blob', 'disc', 'carve', 'shell', 'ellipse')}
    ns.update(math=math, np=np)
    return ns


def _paste(M, pid, T, key, pidx):
    nz = np.argwhere(T.a)
    if not len(nz):
        return
    p = nz + T.o - M.o
    M.a[p[:, 0], p[:, 1], p[:, 2]] = M.kid(key)
    pid[p[:, 0], p[:, 1], p[:, 2]] = pidx


def _execute(ops, boxes, report):
    names = list(boxes) + [f'raw#{i + 1}' for i in range(sum(o.code is not None for o in ops))]
    lo = [min(b[0][i] for b in boxes.values()) for i in range(3)]
    hi = [max(b[1][i] for b in boxes.values()) for i in range(3)]
    M = shapes.Grid()
    M.ensure(lo, hi)
    pid = np.zeros(M.a.shape, np.int32)
    labels = {}
    for op in ops:
        if op.code is None:
            T = shapes.Grid()
            getattr(shapes, op.fn)(T, *op.args, **op.kwargs)
            labels.setdefault(op.part, len(labels) + 1)
            _paste(M, pid, T, op.key, labels[op.part])
        else:
            labels.setdefault(op.part, len(labels) + 1)
            before, o0, shp = M.a.copy(), M.o, M.a.shape
            ns = _raw_namespace()
            ns['V'] = M
            try:
                exec(op.code, ns)
            except Exception as e:
                report.errors.append(f'{op.part}: raw block failed: {type(e).__name__}: {e}')
                continue
            if M.o != o0 or M.a.shape != shp:
                off = tuple(a - b for a, b in zip(o0, M.o))
                sl = tuple(slice(o, o + s) for o, s in zip(off, shp))
                nb, npid = np.zeros(M.a.shape, before.dtype), np.zeros(M.a.shape, np.int32)
                nb[sl], npid[sl] = before, pid
                before, pid = nb, npid
            pid[(M.a != before)] = labels[op.part]
            pid[M.a == 0] = 0
    return M, pid, {v: k for k, v in labels.items()}


def _components(M):
    idx = {tuple(p) for p in np.argwhere(M.a)}
    comps, seen = [], set()
    for start in idx:
        if start in seen:
            continue
        seen.add(start)
        stack, comp = [start], [start]
        while stack:
            x, y, z = stack.pop()
            for d in ((1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1)):
                q = (x + d[0], y + d[1], z + d[2])
                if q in idx and q not in seen:
                    seen.add(q)
                    stack.append(q)
                    comp.append(q)
        comps.append(comp)
    return comps


def _validate(spec, M, pid, pnames, palette, report):
    nz = np.argwhere(M.a)
    if not len(nz):
        report.errors.append('model is empty')
        return
    lo, hi = nz.min(0), nz.max(0)
    report.bounds = (tuple(int(v) for v in lo + M.o), tuple(int(v) for v in hi + M.o))
    report.size = tuple(int(v) for v in hi - lo + 1)
    for ax, (got, want) in enumerate(zip(report.size, spec.voxels)):
        if abs(got - want) > TOLERANCE:
            report.errors.append(f'size {"xyz"[ax]}: model is {got} voxels, Spec says {want} (tolerance {TOLERANCE})')
    for i, k in enumerate(M.keys):
        report.counts[k] = int(np.count_nonzero(M.a == i + 1))
        if k not in palette:
            report.errors.append(f'unknown palette key "{k}"')
    report.counts = {k: v for k, v in report.counts.items() if v}
    comps = _components(M)
    if len(comps) > 1:
        comps.sort(key=len, reverse=True)
        for comp in comps[1:]:
            who = sorted({pnames.get(int(pid[p]), '?') for p in comp})
            report.errors.append(f'floating Part(s) {who}: {len(comp)} voxels not connected to the main body')
    pairs = set()
    for ax in range(3):
        a = np.moveaxis(M.a, ax, 0)
        p = np.moveaxis(pid, ax, 0)
        m = (a[1:] != 0) & (a[:-1] != 0) & (a[1:] == a[:-1]) & (p[1:] != p[:-1]) & (p[1:] != 0) & (p[:-1] != 0)
        for u, v, k in zip(p[1:][m], p[:-1][m], a[1:][m]):
            pairs.add((tuple(sorted((pnames.get(int(u), '?'), pnames.get(int(v), '?')))), M.keys[int(k) - 1]))
    for (x, y), k in sorted(pairs):
        report.warnings.append(f'adjacent Parts "{x}" and "{y}" share colour "{k}"')


def _ascii(M, report):
    nz = np.argwhere(M.a)
    lo, hi = nz.min(0), nz.max(0)
    A = M.a[lo[0]:hi[0] + 1, lo[1]:hi[1] + 1, lo[2]:hi[2] + 1]
    chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz'
    legend = {k: chars[i % len(chars)] for i, k in enumerate(M.keys)}
    lut = np.array(['.'] + [legend[k] for k in M.keys])

    def first(arr, axis, rev):
        arr = np.flip(arr, axis) if rev else arr
        idx = (arr != 0).argmax(axis)
        return np.take_along_axis(arr, np.expand_dims(idx, axis), axis).squeeze(axis)

    views = {'front (x across, z up, seen from -y)': first(A, 1, False),     # [x, z]
             'side (y across, z up, seen from +x)': first(A, 0, True),        # [y, z]
             'top (x across, y down, seen from above)': first(A, 2, True)}    # [x, y]
    for name, v in views.items():
        txt = lut[v]
        rows = txt.T if 'top' in name else txt.T[::-1]              # top view: front (-y) at the top
        report.slices[name] = '\n'.join(''.join(r) for r in rows)
    report.legend = {k: c for k, c in legend.items() if report.counts.get(k)}


def build_model(element_dir, palette=None):
    d = Path(element_dir)
    spec = load_spec(d / 'spec.md')
    if palette is None:
        palette, _ = _find_palette(d)
    report = Report()
    layout = tomllib.loads((d / 'layout.toml').read_text())
    boxes, ops = _resolve(spec, layout, report)
    for name, sp in spec.parts.items():
        if name not in layout:
            report.warnings.append(f'Spec Part "{name}" has no Layout entry')
    for name in boxes:
        c = spec.parts[name].color
        if not c:
            report.errors.append(f'Part "{name}": no colour key in the Spec')
    if report.errors or not boxes:
        return Model(spec, shapes.Grid(), report, ops, palette)
    boxes = _shift(ops, boxes)
    report.parts = {k: (tuple(b[0]), tuple(b[1])) for k, b in boxes.items()}
    raws = layout.get('raw', [])
    for i, r in enumerate(raws):
        ops.append(Op(f'raw#{i + 1}', 'raw', (), {}, code=str(r.get('code', ''))))
    report.raw_count = len(raws)
    M, pid, pnames = _execute(ops, boxes, report)
    _validate(spec, M, pid, pnames, palette, report)
    if report.bounds:
        _ascii(M, report)
    return Model(spec, M, report, ops, palette)


def compile(element_dir, palette=None):
    """Public entry point: returns (voxel Grid, Report) for the Element folder (spec.md + layout.toml)."""
    m = build_model(element_dir, palette)
    return m.grid, m.report
