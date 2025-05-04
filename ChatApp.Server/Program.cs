using ChatApp.Server.Services;
using ChatApp.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

builder.Services.AddControllers();

builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<CloudinaryService>();

builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

var app = builder.Build();

app.MapControllers();
app.MapHub<StatusAccountHub>("/socket/status");
app.MapHub<ChatOneOnOneHub>("/socket/chat-single");

app.Run();
