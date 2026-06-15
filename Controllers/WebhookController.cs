using Microsoft.AspNetCore.Mvc;
using WebhookForTG.Models;
using WebhookForTG.Services;

namespace WebhookForTG.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    // private readonly ILineNotifyService _lineService;
    private readonly ITelegramService _telegramService;
    private readonly ILogger<WebhookController> _logger;
    private readonly string? _secretKey;

    public WebhookController(
        // ILineNotifyService lineService,
        ITelegramService telegramService,
        IConfiguration configuration,
        ILogger<WebhookController> logger)
    {
        // _lineService = lineService;
        _telegramService = telegramService;
        _logger = logger;
        _secretKey = configuration["Webhook:SecretKey"];
    }

    /// <summary>
    /// 接收 webhook 訊息並發送到 Telegram
    /// </summary>
    /// <remarks>
    /// 支援 Header: X-Webhook-Secret 做簡易驗證（可選）
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] WebhookRequest request)
    {
        if (!IsAuthorized())
            return Unauthorized(new { error = "Invalid or missing secret key." });

        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Message cannot be empty." });

        _logger.LogInformation("收到 Webhook 訊息，來源: {Source}", request.Source ?? "unknown");

        var formattedMessage = FormatMessage(request);

        // var lineTask = _lineService.SendMessageAsync(formattedMessage);
        var (tgSuccess, tgError) = await _telegramService.SendMessageAsync(formattedMessage);

        var result = new NotifyResult
        {
            // LineSuccess = lineSuccess,
            TelegramSuccess = tgSuccess,
            // LineError = lineError,
            TelegramError = tgError
        };

        if (!tgSuccess)
        {
            _logger.LogWarning("Telegram 發送失敗: {Error}", tgError);
            return StatusCode(207, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 簡易文字訊息端點（直接用 query string）
    /// </summary>
    [HttpPost("simple")]
    public async Task<IActionResult> Simple([FromQuery] string message, [FromQuery] string? title = null)
    {
        if (!IsAuthorized())
            return Unauthorized(new { error = "Invalid or missing secret key." });

        if (string.IsNullOrWhiteSpace(message))
            return BadRequest(new { error = "Message cannot be empty." });

        var request = new WebhookRequest { Message = message, Title = title };
        var formattedMessage = FormatMessage(request);

        // var lineTask = _lineService.SendMessageAsync(formattedMessage);
        var (tgSuccess, tgError) = await _telegramService.SendMessageAsync(formattedMessage);

        var result = new NotifyResult
        {
            // LineSuccess = lineSuccess,
            TelegramSuccess = tgSuccess,
            // LineError = lineError,
            TelegramError = tgError
        };

        return tgSuccess ? Ok(result) : StatusCode(207, result);
    }

    private bool IsAuthorized()
    {
        if (string.IsNullOrWhiteSpace(_secretKey))
            return true;

        Request.Headers.TryGetValue("X-Webhook-Secret", out var headerSecret);
        return headerSecret == _secretKey;
    }

    private static string FormatMessage(WebhookRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title))
            return $"[{request.Title}]\n{request.Message}";

        return request.Message;
    }
}
