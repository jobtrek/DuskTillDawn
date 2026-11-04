"""Regenerate the Asset table in docs/3d-assets.md from every game-assets/**/spec.md.

Usage: uv run scripts/build_index.py          (rewrite the tables)
       uv run scripts/build_index.py --check  (exit 1 if specs are over 30 lines or the doc is stale)
"""
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DOC = ROOT / "docs" / "3d-assets.md"
MAX_LINES = 30


def load(path):
    text = path.read_text()
    fm_text = text.split("---")[1]
    fm, key = {}, None
    for line in fm_text.strip().splitlines():
        if line.startswith("  ") and key:
            k, v = line.strip().split(": ", 1)
            fm[key][k] = v
        else:
            k, _, v = line.partition(":")
            key = k.strip()
            fm[key] = v.strip() if v.strip() else {}
    parts = {"1": [], "2": [], "3": [], "2/3": []}
    for m in re.finditer(r"^- \[(1|2|3|2/3)\] (.*)$", text, re.M):
        parts[m.group(1)].append(re.split(r" \d", m.group(2))[0])  # drop cm sizes
    fm["_parts"] = parts
    fm["_lines"] = len(text.splitlines())
    fm["_path"] = path.relative_to(ROOT / "game-assets")
    return fm


def size(fm):
    return f"{fm['size_cm'].replace('x', '×')} → {fm['voxels'].replace('x', '×')}"


def link(fm):
    return f"[{fm['id']}](../game-assets/{fm['_path'].as_posix()})"


def row_small(fm):
    states = fm["states"] if isinstance(fm.get("states"), dict) else fm.get("pieces", {})
    st = ", ".join("_" + s for s in states) or "one"
    p = fm["_parts"]
    detail = ", ".join(p["2"]) + " · " + ", ".join(p["3"])
    return f"| {link(fm)} | **{fm['name']}** | {fm['room']} | {size(fm)} | {fm['grid']} | {st} | {detail} | {fm['priority']} |"


def row_room(fm):
    pieces = fm["pieces"] if isinstance(fm.get("pieces"), str) else "one"
    detail = ", ".join(fm["_parts"]["2/3"])
    return f"| {link(fm)} | **{fm['name']}** | {fm['room']} | {size(fm)} | {pieces} | {detail} | {fm['priority']} |"


def table(tier, fms):
    rows = [f for f in fms if f["tier"] == tier]
    if tier in ("T5", "T6"):
        head = ["| ID | Element | Where | Size cm → voxels (W×D×H) | Pieces | Detail | Pri |", "|---|---|---|---|---|---|---|"]
        return head + [row_room(f) for f in rows]
    head = ["| ID | Element | Room | Size cm → voxels (W×D×H) | Grid | States / pieces (separate models) | Secondary · tertiary detail | Pri |", "|---|---|---|---|---|---|---|---|"]
    return head + [row_small(f) for f in rows]


def main():
    fms = sorted((load(p) for p in (ROOT / "game-assets").glob("t*/*/spec.md")), key=lambda f: f["id"])
    errors = [f"{f['id']}: {f['_lines']} lines (max {MAX_LINES})" for f in fms if f["_lines"] > MAX_LINES]
    doc = DOC.read_text()
    new = doc
    for tier in ("T1", "T2", "T3", "T4", "T56"):
        block = "\n".join(table(tier, fms) if tier != "T56" else table("T5", fms) + table("T6", fms)[2:])
        pat = re.compile(rf"(<!-- index:{tier} -->\n).*?(\n<!-- /index:{tier} -->)", re.S)
        if not pat.search(new):
            errors.append(f"marker index:{tier} missing in {DOC.name}")
            continue
        new = pat.sub(lambda m: m.group(1) + block + m.group(2), new)
    if "--check" in sys.argv:
        if new != doc:
            errors.append("docs/3d-assets.md is stale, run build_index.py")
    else:
        DOC.write_text(new)
    print("\n".join(errors) or f"ok: {len(fms)} specs")
    sys.exit(1 if errors else 0)


main()
