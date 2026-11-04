"""Spec rules a machine can check (the old 3d-assets.md rules). Human rules: agent-guide/build-rules.md."""
from pathlib import Path

from .spec import load_spec

MAX_LINES = 40


def lint_spec(path):
    """Return (errors, warnings) for one spec.md. The line cap is a warning: every existing Spec is over it."""
    errors, warnings = [], []
    n = len(Path(path).read_text().splitlines())
    if n > MAX_LINES:
        warnings.append(f'{n} lines (max {MAX_LINES})')
    try:
        spec = load_spec(path)
    except (ValueError, KeyError) as e:
        return [f'spec unreadable: {e}'], warnings
    if spec.grid not in ('S', 'F'):
        errors.append(f'grid is {spec.grid!r}, must be S or F')
    return errors, warnings
