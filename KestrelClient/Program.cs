using System.Diagnostics;
using System.Net;
using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using KestrelCore;
using KestrelServer;
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

//var protocol = new LengthPrefixedProtocol(new DefaultPacketFactoryPool());

var protocol = new FixedHeaderPipelineFilter();

var reader = connection.CreateReader();
var writer = connection.CreateWriter();

var sendCount = 0;
var watch = new Stopwatch();

Console.WriteLine("开始发送数据");

watch.Start();

while (watch.Elapsed.TotalSeconds < 60)
{
    sendCount++;
    var requestMessage = CommandMessage.NewMessage(CommandType.Login, new LoginMessageRequest
    {
        Username = "wujun",
        Password = "ssss",
    });

    await writer.WriteAsync(protocol, requestMessage);

    var result = await reader.ReadAsync(protocol);

    if (result.IsCompleted)
        break;

    reader.Advance();
}

watch.Stop();

Console.WriteLine($"支持完毕总共发送{sendCount}");

Console.ReadKey();