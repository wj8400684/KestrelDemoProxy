using KestrelCore;
using Microsoft.AspNetCore.Connections;

namespace KestrelServer;

public sealed class MessageConnectionHandler(ILogger<MessageConnectionHandler> logger) : ConnectionHandler
{
    private readonly LengthPrefixedProtocol _protocol = new(new DefaultPacketFactoryPool());
    private readonly FixedHeaderPipelineFilter _pipelineFilter = new();

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        logger.LogInformation($"客户端连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");

        var writer = connection.CreateWriter();
        var reader = connection.CreateReader();

        while (!connection.ConnectionClosed.IsCancellationRequested)
        {
            try
            {
                var result = await reader.ReadAsync(_pipelineFilter);

                if (result.IsCompleted)
                    break;
            }
            catch (Exception)
            {
                break;
            }
            finally
            {
                reader.Advance();
            }

            // var responseMessage = CommandPackage.NewMessage(CommandType.LoginReply, new LoginMessageReply
            // {
            //     Token = "ssssssssssss"
            // });
            //
            // await writer.WriteAsync(_pipelineFilter, responseMessage);
        }

        logger.LogInformation($"断开连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");
    }
}