param()

Write-Host "==> Bootstrap: install tools & restore" -ForegroundColor Cyan

# Cài global tools nếu thiếu
$tools = @(
    @{ Name = "dotnet-ef"; Version = "" },
    @{ Name = "dotnet-format"; Version = "" }
)
foreach ($t in $tools) {
    $exists = dotnet tool list -g | Select-String -Pattern "^\s*$($t.Name)\s"
    if (-not $exists) {
        if ([string]::IsNullOrWhiteSpace($t.Version)) {
            dotnet tool install -g $t.Name
        } else {
            dotnet tool install -g $t.Name --version $t.Version
        }
    }
}

# Ensure ~/.dotnet/tools in PATH
$env:PATH = "$env:PATH;$([Environment]::GetFolderPath('UserProfile'))\.dotnet\tools"

# Restore solution
if (Test-Path ./Flex.sln) {
    dotnet restore ./Flex.sln
} else {
    Write-Host "Flex.sln not found, restoring all projects under src/..."
    Get-ChildItem -Path ./src -Filter *.csproj -Recurse | ForEach-Object {
        dotnet restore $_.FullName
    }
}

# Docker check
try {
    docker version | Out-Null
    Write-Host "Docker OK"
} catch {
    Write-Warning "Docker not available. Some targets (run-all with containers) may fail."
}

Write-Host "==> Bootstrap done."
