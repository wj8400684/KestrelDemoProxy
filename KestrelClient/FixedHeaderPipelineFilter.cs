using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Bedrock.Framework.Protocols;
using Google.Protobuf;
using KestrelCore;

namespace KestrelServer;

public struct FixedHeaderPipelineFilter :
    IMessageReader<CommandMessage>,
    IMessageWriter<CommandMessage>
{
    private const int HeaderSize = sizeof(short);

    public bool TryParseMessage(in ReadOnlySequence<byte> input,
        ref SequencePosition consumed,
        ref SequencePosition examined,
        [MaybeNullWhen(false)] out CommandMessage message)
    {
        if (input.IsEmpty)
        {
            message = default;
            return false;
        }
        
        var reader = new SequenceReader<byte>(input);
        if (!reader.TryReadLittleEndian(out short bodyLength) || reader.Remaining < bodyLength)
        {
            message = default;
            return false;
        }

        var payLoad = reader.Sequence.Slice(HeaderSize, bodyLength);

        message = CommandMessage.Parser.ParseFrom(payLoad);

        examined = consumed = payLoad.End;
        return true;
    }

    public void WriteMessage(CommandMessage message, IBufferWriter<byte> output)
    {
        var bodyLength = (short)message.CalculateSize();

        var headSpan = output.GetSpan(HeaderSize);
        BinaryPrimitives.WriteInt16LittleEndian(headSpan, bodyLength);
        output.Advance(HeaderSize);
        
        message.WriteTo(output);
    }
}