"""One-off: convert game-assets/<tier>/<id>.md into game-assets/<tier>/<id>-<Name>/spec.md.

Run once: uv run scripts/migrate_specs.py   (old files are deleted after a backup copy is made)
"""
import re
import shutil
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
ASSETS = ROOT / "game-assets"
BACKUP = Path(sys.argv[1]) if len(sys.argv) > 1 else None


def field(text, label):
    m = re.search(rf"^\| {re.escape(label)} \| (.*) \|$", text, re.M)
    return m.group(1).strip() if m else ""


def section(text, title):
    m = re.search(rf"^## {re.escape(title)}.*?\n(.*?)(?=^## |\Z)", text, re.M | re.S)
    return m.group(1) if m else ""


def migrate(path):
    text = path.read_text()
    sid = path.stem
    name = re.search(r"^# [\w-]+: (\w+)", text).group(1)
    size_cm = field(text, "Real size (W × D × H)").replace(" cm", "").replace(" × ", "x")
    voxels = field(text, "Size in voxels (W × D × H)").replace(" × ", "x")
    grid = field(text, "Grid")[0]
    room = field(text, "Used in") or field(text, "Where")
    fm = [
        "---",
        f"name: {name}",
        f"id: {sid}",
        f"tier: {sid[:2].upper()}",
        f"room: {room}",
        f"priority: {field(text, 'Priority')}",
        f"size_cm: {size_cm}",
        f"voxels: {voxels}",
        f"grid: {grid}",
        f"grain: {field(text, '`grain`')}",
        f"collider: {field(text, '`collider`').lower()}",
    ]
    pieces = section(text, "Pieces")
    if pieces:
        names = re.findall(r"^\| `Room_\w+?_(\w+)` \|", pieces, re.M)
        wall = re.search(r"Wall thickness (\d+) cm", pieces)
        fm.append("pieces: " + ", ".join(names))
        if wall:
            fm.append(f"wall_cm: {wall.group(1)}")
    extra = section(text, "Parts")
    rows = re.findall(r"^\| `\w+?_(\w+)` \| (.*) \|$", extra, re.M)
    if rows:
        fm.append("pieces:")
        fm += [f"  {s}: {d}" for s, d in rows]
    states = re.findall(r"^\| `\w+?_(\w+)` \| (.*) \|$", section(text, "States"), re.M)
    if states:
        fm.append("states:")
        fm += [f"  {s}: {d}" for s, d in states]
    fm.append("---")

    detail = section(text, "Detail layers")
    body = [f"# {name}", ""]
    parts = []
    tag = None
    for line in detail.splitlines():
        if "Secondary parts and tertiary" in line:
            tag = "2/3"
        elif "Secondary parts" in line:
            tag = "2"
        elif "Tertiary detail" in line:
            tag = "3"
        elif line.startswith("   - ") and tag:
            parts.append(f"- [{tag}] {line[5:].strip()}")
    body += ["## Parts (tier; cm size + colour key to fill before first build)", ""]
    body += parts or ["- (none)"]
    notes = [l for l in section(text, "Notes").splitlines() if l.startswith("- ")]
    notes += [f"- {l}" for l in extra.splitlines() if l.strip() and not l.startswith("|")]
    if notes:
        body += ["", "## Notes", "", *notes]

    out = path.parent / f"{sid}-{name}" / "spec.md"
    out.parent.mkdir(exist_ok=True)
    out.write_text("\n".join(fm + [""] + body) + "\n")
    return out


for tier in sorted(ASSETS.glob("t[1-6]-*")):
    for old in sorted(tier.glob("t*.md")):
        if BACKUP:
            (BACKUP / tier.name).mkdir(parents=True, exist_ok=True)
            shutil.copy(old, BACKUP / tier.name / old.name)
        out = migrate(old)
        old.unlink()
        n = len(out.read_text().splitlines())
        print(f"{out.relative_to(ROOT)}  {n} lines" + ("  OVER 30" if n > 30 else ""))
    gk = tier / ".gitkeep"
    gk.unlink(missing_ok=True)
