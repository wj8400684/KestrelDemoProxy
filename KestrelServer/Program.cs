using KestrelServer;
using Microsoft.AspNetCore.Connections;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081, l => l.UseConnectionHandler<MessageConnectionHandler>());
});

var app = builder.Build();

app.Run();