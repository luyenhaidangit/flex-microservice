param(
  [string]$Project,
  [string]$DbContext,
  [string]$MigrationName
)

if (-not $Project) {
    Write-Error "Please pass -Project path to the .csproj (e.g., src/IdentityService/IdentityService.csproj)"
    exit 1
}

if ($MigrationName) {
    if (-not $DbContext) {
        Write-Error "Creating a migration requires -DbContext."
        exit 1
    }
    Write-Host "==> Creating migration '$MigrationName' for $DbContext on $Project" -ForegroundColor Cyan
    dotnet ef migrations add $MigrationName --project $Project --context $DbContext
} else {
    Write-Host "==> Applying migrations for $Project" -ForegroundColor Cyan
    dotnet ef database update --project $Project
}
