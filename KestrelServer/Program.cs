using KestrelServer;
using KestrelServer.Commands;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

// var serviceType = typeof(IAsyncCommand);
// var cmdHandlerTypes = serviceType.Assembly.GetTypes()
//     .Where(item => serviceType.IsAssignableFrom(item))
//     .Where(item => item.IsClass && item.IsAbstract == false);

builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IAsyncCommand, LoginCommand>());

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenNamedPipe("8081", l => l.UseConnectionHandler<CommandConnectionHandler>());
});

var app = builder.Build();

app.Run();