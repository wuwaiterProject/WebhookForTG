using System.Text;
using System.Text.Json;

namespace WebhookForTG.Services;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _httpClient;
    private readonly string _botToken;
    private readonly string _chatId;
    private readonly ILogger<TelegramService> _logger;

    public TelegramService(HttpClient httpClient, IConfiguration configuration, ILogger<TelegramService> logger)
    {
        _httpClient = httpClient;
        _botToken = configuration["Telegram:BotToken"]
            ?? throw new InvalidOperationException("Telegram:BotToken is not configured.");
        _chatId = configuration["Telegram:ChatId"]
            ?? throw new InvalidOperationException("Telegram:ChatId is not configured.");
        _logger = logger;
    }

    public async Task<(bool Success, string? Error)> SendMessageAsync(string message)
    {
        try
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

            var payload = new
            {
                chat_id = _chatId,
                text = message,
                parse_mode = "HTML"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Telegram 發送成功");
                return (true, null);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Telegram 發送失敗: {StatusCode} - {Body}", response.StatusCode, errorBody);
            return (false, $"HTTP {(int)response.StatusCode}: {errorBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Telegram 發送時發生例外");
            return (false, ex.Message);
        }
    }
}
