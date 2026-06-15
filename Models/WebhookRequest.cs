namespace WebhookForTG.Models;

public class WebhookRequest
{
    public string Message { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Source { get; set; }
}
