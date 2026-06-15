namespace WebhookForTG.Services;

public interface ILineNotifyService
{
    Task<(bool Success, string? Error)> SendMessageAsync(string message);
}
