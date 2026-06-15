using System.Net.Http.Headers;

namespace WebhookForTG.Services;

public class LineNotifyService : ILineNotifyService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly ILogger<LineNotifyService> _logger;

    private const string LineNotifyApiUrl = "https://notify-api.line.me/api/notify";

    public LineNotifyService(HttpClient httpClient, IConfiguration configuration, ILogger<LineNotifyService> logger)
    {
        _httpClient = httpClient;
        _accessToken = configuration["LineNotify:AccessToken"]
            ?? throw new InvalidOperationException("LineNotify:AccessToken is not configured.");
        _logger = logger;
    }

    public async Task<(bool Success, string? Error)> SendMessageAsync(string message)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken);

            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("message", message)
            });

            var response = await _httpClient.PostAsync(LineNotifyApiUrl, formData);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("LINE Notify 發送成功");
                return (true, null);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("LINE Notify 發送失敗: {StatusCode} - {Body}", response.StatusCode, errorBody);
            return (false, $"HTTP {(int)response.StatusCode}: {errorBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LINE Notify 發送時發生例外");
            return (false, ex.Message);
        }
    }
}
