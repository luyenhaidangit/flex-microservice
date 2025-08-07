param(
  [string]$Configuration = "Debug",
  [string]$Service       = ""
)

Write-Host "==> Run-all ($Configuration)" -ForegroundColor Cyan

# Ưu tiên docker-compose nếu có
if (Test-Path ./docker-compose.yml) {
    docker compose up -d
    Write-Host "docker compose up -d started."
    exit 0
}

# Nếu không có compose: chạy dotnet run cho từng service (csproj) trong src/
$projects = Get-ChildItem -Path ./src -Recurse -Filter *.csproj |
    Where-Object { $_.FullName -notmatch '\\tests\\' }

if ($Service) {
    $projects = $projects | Where-Object { $_.FullName -match [Regex]::Escape($Service) }
}

if (-not $projects) {
    Write-Warning "No projects found to run."
    exit 1
}

# Mở mỗi service trong 1 pwsh job
foreach ($p in $projects) {
    Write-Host "Starting: $($p.FullName)"
    Start-Job -ScriptBlock {
        param($proj, $cfg)
        dotnet run --project $proj -c $cfg
    } -ArgumentList $p.FullName, $Configuration | Out-Null
}

Write-Host "Services started in background jobs. Use Get-Job to inspect."
