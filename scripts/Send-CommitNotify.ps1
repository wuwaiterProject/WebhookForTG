param(
    [string]$WebhookUrl = "http://localhost:5261/api/webhook"
)

# 取得 commit 資訊
$branch    = git rev-parse --abbrev-ref HEAD
$hash      = git rev-parse --short HEAD
$commitMsg = git log -1 --pretty=%s
$author    = git log -1 --pretty=%an
$repoName  = Split-Path -Leaf (git rev-parse --show-toplevel)

$message = @"
Repo  : <code>$repoName</code>
Branch: <code>$branch</code>
Commit: <code>$hash</code>
Author: $author

$commitMsg
"@

$body = @{
    title   = "Git Commit"
    message = $message
} | ConvertTo-Json

$bytes = [System.Text.Encoding]::UTF8.GetBytes($body)

try {
    Invoke-WebRequest -Uri $WebhookUrl `
        -Method POST `
        -ContentType "application/json; charset=utf-8" `
        -Body $bytes `
        -UseBasicParsing | Out-Null
    Write-Host "[Webhook] Telegram 通知發送成功" -ForegroundColor Green
} catch {
    Write-Host "[Webhook] 通知發送失敗: $_" -ForegroundColor Yellow
}
