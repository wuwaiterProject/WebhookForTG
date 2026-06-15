namespace WebhookForTG.Services;

public interface ITelegramService
{
    Task<(bool Success, string? Error)> SendMessageAsync(string message);
}
