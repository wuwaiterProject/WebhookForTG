using WebhookForTG.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// builder.Services.AddHttpClient<ILineNotifyService, LineNotifyService>();
builder.Services.AddHttpClient<ITelegramService, TelegramService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
