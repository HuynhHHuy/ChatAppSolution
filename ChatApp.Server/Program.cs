using ChatApp.Server.Services;
using ChatApp.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

builder.Services.AddControllers();

builder.Services.AddScoped<EmailService>();

var app = builder.Build();

app.MapControllers();
app.MapHub<StatusAccountHub>("/socket/status");

app.Run();
