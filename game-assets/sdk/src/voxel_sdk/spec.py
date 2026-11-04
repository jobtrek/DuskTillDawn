"""Read an Element's spec.md: front matter plus the Parts list (names, cm sizes, colour keys)."""
import re
from dataclasses import dataclass, field
from pathlib import Path


@dataclass
class SpecPart:
    name: str
    size_cm: tuple | None      # 1 to 3 numbers in x, y, z order, or None when the line has no size
    color: str | None


@dataclass
class Spec:
    name: str
    id: str
    grid: str                  # 'S' (1/64 m) or 'F' (1/128 m)
    size_cm: tuple
    voxels: tuple
    grain: float
    states: list = field(default_factory=list)    # State names in Spec order; the first is the base State
    parts: dict = field(default_factory=dict)
    text: str = ''

    @property
    def cm_per_voxel(self):
        return 100 / 64 if self.grid == 'S' else 100 / 128


_NUM = r'\d+(?:\.\d+)?'
_SIZE = re.compile(rf'({_NUM}(?:\s*x\s*{_NUM})*)\s*cm')


def _triple(s):
    return tuple(float(n) for n in re.findall(_NUM, s))


def parse_part_line(line):
    """'- [2] bezel: 54 x 5 x 28 cm, plastic_black' -> SpecPart. Returns None for non-Part lines."""
    m = re.match(r'-\s*(?:\[\d\]\s*)?(.+)$', line.strip())
    if not m:
        return None
    body = m.group(1)
    head, sep, rest = body.partition(':')
    if not sep:                                    # 'chassis 45 x 60 x 4.4 cm, chassis'
        mm = re.search(rf'\s{_NUM}', body)
        head, rest = (body[:mm.start()], body[mm.start():]) if mm else (body, '')
    name = head.strip()
    size = _SIZE.search(rest)
    color = None
    if ',' in rest:
        tail = rest.rsplit(',', 1)[1].split()
        color = tail[0].strip('`;.') if tail else None
    return SpecPart(name, _triple(size.group(1)) if size else None, color)


def load_spec(path):
    text = Path(path).read_text()
    fm = re.match(r'---\n(.*?)\n---', text, re.S)
    if not fm:
        raise ValueError(f'{path}: no front matter')
    kv = dict(re.findall(r'^(\w+):\s*(.*)$', fm.group(1), re.M))
    dims = lambda s: tuple(float(n) for n in s.split('x'))
    spec = Spec(name=kv['name'], id=kv['id'], grid=kv.get('grid', 'S').strip().upper(),
                size_cm=dims(kv['size_cm']), voxels=tuple(int(n) for n in kv['voxels'].split('x')),
                grain=float(kv.get('grain', 0)), text=text.rstrip())
    flat = kv.get('states', '').strip()
    st = re.search(r'^states:[ \t]*\n((?:[ \t]+\S.*\n?)+)', fm.group(1) + '\n', re.M)
    if flat.startswith('['):                       # states: [On, Off]
        spec.states = [x.strip() for x in flat.strip('[]').split(',') if x.strip()]
    elif st:                                        # one 'Name: note' line per State, at the first indent level
        lines = st.group(1).splitlines()
        ind = min(len(l) - len(l.lstrip()) for l in lines)
        spec.states = [l.strip().split(':', 1)[0].strip() for l in lines
                       if len(l) - len(l.lstrip()) == ind and ':' in l]
    sect = re.search(r'^## Parts[^\n]*\n(.*?)(?=^## |\Z)', text, re.M | re.S)
    for line in (sect.group(1).splitlines() if sect else []):
        part = parse_part_line(line) if line.lstrip().startswith('-') else None
        if part:
            spec.parts[part.name] = part
    return spec
