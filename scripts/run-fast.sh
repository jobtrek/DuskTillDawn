#!/usr/bin/env bash
# Launch the game with the in-game clock sped up (default 20x) to watch the sky/sun/moon cycle.
# Usage: scripts/run-fast.sh [speed]    e.g. scripts/run-fast.sh 50
set -euo pipefail

SPEED="${1:-20}"
GODOT="${GODOT:-/Applications/Godot_mono.app/Contents/MacOS/Godot}"
PROJECT_DIR="$(cd "$(dirname "$0")/../dusk-till-dawn" && pwd)"

dotnet build "$PROJECT_DIR/DuskTillDawn.csproj" -v quiet
exec "$GODOT" --path "$PROJECT_DIR" -- --clock-speed="$SPEED"
