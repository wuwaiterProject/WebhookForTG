namespace WebhookForTG.Models;

public class NotifyResult
{
    // public bool LineSuccess { get; set; }
    public bool TelegramSuccess { get; set; }
    // public string? LineError { get; set; }
    public string? TelegramError { get; set; }
}
