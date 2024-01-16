using KestrelCore;
using Microsoft.AspNetCore.Connections;

namespace KestrelServer.Server;

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
