$repoRoot = git rev-parse --show-toplevel
$hooksDir = Join-Path $repoRoot "hooks"
$gitHooks = Join-Path $repoRoot ".git\hooks"

Get-ChildItem $hooksDir | ForEach-Object {
    $dest = Join-Path $gitHooks $_.Name
    Copy-Item $_.FullName $dest -Force
    Write-Host "Installed: $($_.Name)" -ForegroundColor Green
}

Write-Host "`nGit hooks installed!" -ForegroundColor Cyan