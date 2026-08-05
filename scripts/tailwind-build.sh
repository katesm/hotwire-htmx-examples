#!/usr/bin/env bash
set -euo pipefail

project_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
tailwind="$project_root/tools/tailwindcss"

if [[ ! -x "$tailwind" ]]; then
  echo "Tailwind executable not found. Run ./scripts/install-tailwind.sh first." >&2
  exit 1
fi

"$tailwind" --config "$project_root/tailwind.config.js" -i "$project_root/Styles/app.css" -o "$project_root/wwwroot/css/site.css" --minify
