$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$version = "3.4.17"
$toolsDirectory = Join-Path $projectRoot "tools"
$tailwind = Join-Path $toolsDirectory "tailwindcss.exe"

New-Item -ItemType Directory -Force -Path $toolsDirectory | Out-Null
Invoke-WebRequest `
    -Uri "https://github.com/tailwindlabs/tailwindcss/releases/download/v$version/tailwindcss-windows-x64.exe" `
    -OutFile $tailwind

& (Join-Path $PSScriptRoot "tailwind-build.ps1")
