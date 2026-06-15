---
name: add-notification-provider
description: 為 WebhookForTG 新增通知平台（如 Slack、Discord、Teams 等）。當使用者說要新增通知管道、整合新的訊息平台、或詢問如何擴充通知時使用此 skill。
disable-model-invocation: true
---

# 新增通知平台 Skill

## 新增步驟

依序完成以下 5 個步驟：

### 步驟 1：appsettings.json 新增設定 Section

```json
"{PlatformName}": {
  "Token": "YOUR_{PLATFORM}_TOKEN",
  "TargetId": "YOUR_TARGET_ID"
}
```

### 步驟 2：建立介面 `Services/I{Platform}Service.cs`

```csharp
namespace WebhookForTG.Services;

public interface I{Platform}Service
{
    Task<(bool Success, string? Error)> SendMessageAsync(string message);
}
```

### 步驟 3：建立實作 `Services/{Platform}Service.cs`

複製 `TelegramService.cs` 作為起點，修改：
- 類別名稱與建構子
- `appsettings` 讀取的 key 路徑
- API URL 與 request payload 格式

必須保留的模式：
- `try/catch` 包住 HTTP 呼叫
- 成功回傳 `(true, null)`，失敗回傳 `(false, errorMessage)`
- 成功用 `LogInformation`，失敗用 `LogWarning` 或 `LogError`

### 步驟 4：Program.cs 注冊

在現有 `AddHttpClient` 行後面新增：

```csharp
builder.Services.AddHttpClient<I{Platform}Service, {Platform}Service>();
```

### 步驟 5：WebhookController.cs 整合

1. 建構子注入 `I{Platform}Service`
2. 在 `Receive()` 與 `Simple()` 方法中加入新 Task：

```csharp
var {platform}Task = _{platform}Service.SendMessageAsync(formattedMessage);
await Task.WhenAll(lineTask, telegramTask, {platform}Task);
```

3. 更新 `NotifyResult` Model 新增對應的 `{Platform}Success` 與 `{Platform}Error` 屬性

## 常用平台 API 參考

### Slack Incoming Webhook
```
POST https://hooks.slack.com/services/...
Content-Type: application/json
Body: { "text": "message" }
```

### Discord Webhook
```
POST https://discord.com/api/webhooks/{id}/{token}
Content-Type: application/json
Body: { "content": "message" }
```

### Microsoft Teams
```
POST {incoming_webhook_url}
Content-Type: application/json
Body: { "text": "message" }
```
