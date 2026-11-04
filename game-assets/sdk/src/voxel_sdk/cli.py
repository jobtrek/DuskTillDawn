"""voxel-sdk check <dir> | build <dir>"""
import sys

from .core import compile
from .generate import write_build


def main(argv=None):
    argv = sys.argv[1:] if argv is None else argv
    if len(argv) != 2 or argv[0] not in ('check', 'build'):
        print('usage: voxel-sdk check|build <element-dir>')
        return 2
    cmd, d = argv
    if cmd == 'check':
        _, report = compile(d)
        print(report)
        return 0 if report.ok else 1
    try:
        report = write_build(d)
    except ValueError as e:
        print(e)
        return 1
    print(report)
    print(f'wrote {d}/build.py')
    return 0
