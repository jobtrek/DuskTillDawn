"""Generate the pixel-art textures for the desk computer UI (no dependencies).

Run from the repo root:  python3 scripts/gen_computer_ui_textures.py
Writes PNGs into dusk-till-dawn/main/computer_ui/textures/.
Godot shows them with nearest filtering, so every texel stays a crisp block.
"""

import math
import struct
import zlib
from pathlib import Path

OUT = Path(__file__).resolve().parent.parent / "dusk-till-dawn/main/computer_ui/textures"


def hex_rgba(h: str, a: int = 255) -> tuple[int, int, int, int]:
    h = h.lstrip("#")
    return int(h[0:2], 16), int(h[2:4], 16), int(h[4:6], 16), a


def write_png(path: Path, pixels: list[list[tuple[int, int, int, int]]]) -> None:
    h, w = len(pixels), len(pixels[0])
    raw = b"".join(b"\x00" + b"".join(struct.pack("4B", *p) for p in row) for row in pixels)

    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data))

    png = b"\x89PNG\r\n\x1a\n"
    png += chunk(b"IHDR", struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0))
    png += chunk(b"IDAT", zlib.compress(raw, 9))
    png += chunk(b"IEND", b"")
    path.write_bytes(png)


def from_ascii(art: str, palette: dict[str, tuple[int, int, int, int]]) -> list[list[tuple[int, int, int, int]]]:
    rows = [r for r in art.strip("\n").split("\n")]
    assert len(rows) == 16 and all(len(r) == 16 for r in rows), "icons must be 16x16"
    return [[palette[c] for c in r] for r in rows]


def upscale(pixels, k: int):
    return [[p for p in row for _ in range(k)] for row in pixels for _ in range(k)]


CLEAR = (0, 0, 0, 0)
INK = hex_rgba("1b1b2a")

MAIL = """
................
................
.............rr.
kkkkkkkkkkkkkrrk
kwgwwwwwwwwwwgwk
kgwgwwwwwwwwgwgk
kwwwgwwwwwwgwwwk
kwwwwgwwwwgwwwwk
kwwwwwgwwgwwwwwk
kwwwwwwggwwwwwwk
kwwwwwwwwwwwwwwk
kwwwwwwwwwwwwwwk
kggggggggggggggk
kkkkkkkkkkkkkkkk
................
................
"""

MONITORING = """
................
.kkkkkkkkkkkkkk.
.kddddddddddddk.
.kddddddddddGdk.
.kdddddddddGddk.
.kddGdddddGdddk.
.kdGdGdddGddddk.
.kGdddGdGdddddk.
.kddddddGdddddk.
.kddddddddddddk.
.kkkkkkkkkkkkkk.
......kssk......
......kssk......
....kkkkkkkk....
................
................
"""

CHAT = """
................
..kkkkkkkkkkkk..
.kttttttttttttk.
.kttttttwtttttk.
.ktttttwwwttttk.
.kttttwwwwwtttk.
.ktttttwwwttttk.
.kttttttwtttttk.
.kttttttttttttk.
..kkkkttkkkkkk..
.....ktk........
.....kk.........
................
................
................
................
"""


def icons() -> None:
    base = {".": CLEAR, "k": INK}
    write_png(OUT / "icon_mail.png", from_ascii(MAIL, {
        **base, "w": hex_rgba("f4f1e8"), "g": hex_rgba("b9b4c7"), "r": hex_rgba("e0304a"),
    }))
    write_png(OUT / "icon_monitoring.png", from_ascii(MONITORING, {
        **base, "d": hex_rgba("0d1f14"), "G": hex_rgba("3cff6e"), "s": hex_rgba("8a8f9c"),
    }))
    write_png(OUT / "icon_chat.png", from_ascii(CHAT, {
        **base, "t": hex_rgba("10a37f"), "w": hex_rgba("ffffff"),
    }))


BAYER4 = [[0, 8, 2, 10], [12, 4, 14, 6], [3, 11, 1, 9], [15, 7, 13, 5]]


def wallpaper() -> None:
    """160x90 Debian-style desktop: dithered navy-to-blue sky, soft bands, red pixel swirl."""
    w, h = 160, 90
    shades = [hex_rgba(c) for c in ("07142b", "0b1f42", "10305f", "17437f", "1f579c", "2a6bb4")]
    px = [[CLEAR] * w for _ in range(h)]
    for y in range(h):
        for x in range(w):
            # Diagonal gradient plus a gentle wave, quantised with ordered dithering.
            t = (y / h) * 0.75 + (x / w) * 0.25 + 0.08 * math.sin(x / 13 + y / 22)
            v = max(0.0, min(0.999, t)) * (len(shades) - 1)
            i = int(v)
            if v - i > BAYER4[y % 4][x % 4] / 16 and i + 1 < len(shades):
                i += 1
            px[y][x] = shades[i]

    # Debian-red swirl (Archimedean spiral), thicker toward the outside.
    red, dark_red = hex_rgba("d70751"), hex_rgba("8f0436")
    cx, cy = 112, 46
    theta = 0.6
    while theta < 4.6 * math.pi:
        r = 2.2 * theta
        x, y = cx + r * math.cos(theta), cy + r * math.sin(theta)
        thick = 1 if theta < 2 * math.pi else 2
        for dx in range(-thick, thick + 1):
            for dy in range(-thick, thick + 1):
                if dx * dx + dy * dy <= thick * thick:
                    px[int(y + dy)][int(x + dx)] = red
        px[int(y) + thick][int(x) + 1] = dark_red
        theta += 0.04

    write_png(OUT / "wallpaper.png", px)


def frame() -> None:
    """12x12 parchment map border (Minecraft-map style), upscaled 4x -> 48x48, 16px nine-patch margins."""
    edge, paper, speck, inner = hex_rgba("5a3a1c"), hex_rgba("e6cfa0"), hex_rgba("cfb27a"), hex_rgba("8a6236")
    n = 12
    px = [[CLEAR] * n for _ in range(n)]
    for y in range(n):
        for x in range(n):
            d = min(x, y, n - 1 - x, n - 1 - y)
            if d == 0:
                px[y][x] = edge
            elif d in (1, 2):
                px[y][x] = speck if (x * 7 + y * 3) % 5 == 0 else paper
            elif d == 3:
                px[y][x] = inner
    # Rounded outer corners.
    for x, y in ((0, 0), (n - 1, 0), (0, n - 1), (n - 1, n - 1)):
        px[y][x] = CLEAR
    write_png(OUT / "frame.png", upscale(px, 4))


if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    icons()
    wallpaper()
    frame()
    print(f"wrote textures to {OUT}")
