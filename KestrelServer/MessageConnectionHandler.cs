using KestrelCore;
using Microsoft.AspNetCore.Connections;

namespace KestrelServer;

public sealed class MessageConnectionHandler(ILogger<MessageConnectionHandler> logger) : ConnectionHandler
{
    private readonly LengthPrefixedProtocol _protocol = new(new DefaultPacketFactoryPool());

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        logger.LogInformation($"客户端连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");

        var writer = connection.CreateWriter();
        var reader = connection.CreateReader();

        while (true)
        {
            try
            {
                var result = await reader.ReadAsync(_protocol);

                if (result.IsCompleted)
                    break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                reader.Advance();
            }

            await writer.WriteAsync(_protocol, new LoginResponse
            {
                SuccessFul = true,
            });
        }
        
        logger.LogInformation($"断开连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");
    }
}