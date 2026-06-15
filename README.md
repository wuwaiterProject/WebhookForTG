# WebhookForTG

接收 Webhook 訊息並同步轉發到 **LINE Notify** 與 **Telegram Bot**。

## 技術規格

- .NET 8 Web API
- 無外部 NuGet 套件相依（僅使用內建 `HttpClient`）

---

## 快速開始

### 1. 取得必要 Token

#### LINE Notify
1. 前往 [LINE Notify 官網](https://notify-bot.line.me/)
2. 登入後點選「個人頁面」→「發行存取權杖」
3. 輸入服務名稱，選擇要發送的聊天群組，複製產生的 Token

#### Telegram Bot
1. 在 Telegram 搜尋 `@BotFather`，輸入 `/newbot` 建立機器人
2. 複製取得的 **Bot Token**（格式：`123456789:ABCDefgh...`）
3. 取得 **Chat ID**：
   - 將 Bot 加入目標群組（或直接與 Bot 私訊）
   - 瀏覽 `https://api.telegram.org/bot<YOUR_TOKEN>/getUpdates`
   - 在回傳 JSON 中找到 `chat.id`

### 2. 設定 appsettings.json

```json
{
  "LineNotify": {
    "AccessToken": "你的 LINE Notify Token"
  },
  "Telegram": {
    "BotToken": "你的 Telegram Bot Token",
    "ChatId": "你的 Chat ID"
  },
  "Webhook": {
    "SecretKey": "自訂的驗證金鑰（可留空不驗證）"
  }
}
```

### 3. 執行

```bash
dotnet run
```

預設會在 `http://localhost:5000` 啟動。

---

## API 端點

### POST `/api/webhook`

JSON Body 發送：

```json
{
  "message": "你好，這是一則測試訊息",
  "title": "系統通知",
  "source": "MyApp"
}
```

| 欄位 | 必填 | 說明 |
|------|------|------|
| `message` | ✅ | 訊息內容 |
| `title` | ❌ | 標題，若有則顯示為 `[標題]\n訊息` |
| `source` | ❌ | 來源標識（僅記錄 log） |

---

### POST `/api/webhook/simple`

Query String 發送（適合簡易串接）：

```
POST /api/webhook/simple?message=測試訊息&title=通知
```

---

## 驗證機制

若 `appsettings.json` 中有設定 `Webhook:SecretKey`，請求需帶上 Header：

```
X-Webhook-Secret: 你的金鑰
```

若 `SecretKey` 為空，則不需要驗證。

---

## 回傳格式

```json
{
  "lineSuccess": true,
  "telegramSuccess": true,
  "lineError": null,
  "telegramError": null
}
```

- 兩者皆成功 → HTTP `200`
- 部分失敗 → HTTP `207 Multi-Status`

---

## cURL 範例

```bash
# JSON 格式
curl -X POST http://localhost:5000/api/webhook \
  -H "Content-Type: application/json" \
  -H "X-Webhook-Secret: YOUR_SECRET" \
  -d '{"message":"伺服器 CPU 超過 90%","title":"警告","source":"monitoring"}'

# 簡易格式
curl -X POST "http://localhost:5000/api/webhook/simple?message=Hello&title=Test" \
  -H "X-Webhook-Secret: YOUR_SECRET"
```
