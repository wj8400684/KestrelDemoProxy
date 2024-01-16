using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using KestrelCore;
using KestrelServer.Commands;
using Microsoft.AspNetCore.Connections;

namespace KestrelServer;




public sealed class AppChannel(ConnectionContext connection)
{
    private readonly FixedHeaderPipelineFilter _pipelineFilter = new();
    private readonly ProtocolReader _reader = connection.CreateReader();
    private readonly ProtocolWriter _writer = connection.CreateWriter();

    public async ValueTask<CommandMessage?> ReadAsync(CancellationToken cancellationToken = default)
    {
        ProtocolReadResult<CommandMessage> result;

        try
        {
            result = await _reader.ReadAsync(_pipelineFilter, cancellationToken);
        }
        finally
        {
            _reader.Advance();
        }

        return result.Message;
    }

    public ValueTask WriterAsync(CommandMessage message,
        CancellationToken cancellationToken = default)
    {
        return _writer.WriteAsync(_pipelineFilter, message, cancellationToken);
    }
}

public class CommandBuilder
{
}

public sealed class CommandConnectionHandler(ILogger<CommandConnectionHandler> logger,
    IEnumerable<IAsyncCommand> commands) : ConnectionHandler
{
    private readonly ReadOnlyDictionary<CommandType, IAsyncCommand> _commands =
        new(commands.ToDictionary(item => item.CommandType, item => item));

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        logger.LogInformation($"客户端连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");

        var channel = new AppChannel(connection);

        while (!connection.ConnectionClosed.IsCancellationRequested)
        {
            var message = await channel.ReadAsync();

            if (message == null)
                continue;

            try
            {
                if (_commands.TryGetValue(message.Key, out var command))
                    await command.ExecuteAsync(channel, message);
            }
            catch (Exception e)
            {
                logger.LogError($"执行命令{message.Key}发生异常：{connection.ConnectionId}-{connection.RemoteEndPoint}", e);
            }
        }

        logger.LogInformation($"断开连接：{connection.ConnectionId}-{connection.RemoteEndPoint}");
    }
}