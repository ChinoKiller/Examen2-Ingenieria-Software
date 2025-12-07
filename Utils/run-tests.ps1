# run-tests.ps1
# Ejecuta la suite de pruebas del backend

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Write-Host "Ejecutando pruebas desde $repoRoot" -ForegroundColor Yellow

Set-Location $repoRoot

dotnet test .\Tests\Backend.Tests\Backend.Tests.csproj
