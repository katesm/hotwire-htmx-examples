#!/usr/bin/env bash
set -euo pipefail

project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
version="3.4.17"
os="$(uname -s)"
arch="$(uname -m)"

case "$os-$arch" in
  Darwin-arm64) asset="tailwindcss-macos-arm64" ;;
  Darwin-x86_64) asset="tailwindcss-macos-x64" ;;
  Linux-x86_64) asset="tailwindcss-linux-x64" ;;
  Linux-aarch64) asset="tailwindcss-linux-arm64" ;;
  *) echo "Unsupported platform: $os-$arch" >&2; exit 1 ;;
esac

mkdir -p "$project_root/tools"
curl --fail --location --output "$project_root/tools/tailwindcss" \
  "https://github.com/tailwindlabs/tailwindcss/releases/download/v$version/$asset"
chmod +x "$project_root/tools/tailwindcss"
"$project_root/scripts/tailwind-build.sh"
