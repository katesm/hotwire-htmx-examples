$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$tailwind = Join-Path $projectRoot "tools/tailwindcss.exe"

if (-not (Test-Path $tailwind)) {
    throw "Tailwind executable not found. Run .\scripts\install-tailwind.ps1 first."
}

& $tailwind --config (Join-Path $projectRoot "tailwind.config.js") -i (Join-Path $projectRoot "Styles/app.css") -o (Join-Path $projectRoot "wwwroot/css/site.css") --minify
