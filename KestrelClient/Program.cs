using System.Diagnostics;
using System.Net;
using System.Text;
using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using KestrelClient;
using KestrelCore;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
    builder.AddConsole();
});

var client = new ClientBuilder()
    .UseSockets()
    .Build();

await using var connection = await client.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 8081));
Console.WriteLine($"Connected to {connection.LocalEndPoint}");

var protocol = new LengthPrefixedProtocol(new DefaultPacketFactoryPool());

var reader = connection.CreateReader();
var writer = connection.CreateWriter();

var sendCount = 0;
var watch = new Stopwatch();

Console.WriteLine("开始发送数据");

watch.Start();

while (watch.Elapsed.TotalSeconds < 60)
{
    sendCount++;
    await writer.WriteAsync(protocol, new LoginRequest
    {
        Username = "wujun",
        Password = "ssss",
    });
    var result = await reader.ReadAsync(protocol);

    if (result.IsCompleted)
        break;

    reader.Advance();
}

watch.Stop();

Console.WriteLine($"支持完毕总共发送{sendCount}");

Console.ReadKey();