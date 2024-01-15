using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Bedrock.Framework.Protocols;
using KestrelCore;

namespace KestrelClient;

public sealed class LengthPrefixedProtocol(IPacketFactoryPool packetFactoryPool) :
    IMessageReader<MessageBase>,
    IMessageWriter<MessageBase>
{
    private const byte HeaderSize = sizeof(short);

    public bool TryParseMessage(in ReadOnlySequence<byte> input,
        ref SequencePosition consumed,
        ref SequencePosition examined,
        out MessageBase message)
    {
        if (input.Length == 0)
        {
            message = default;
            return false;
        }

        var reader = new SequenceReader<byte>(input);
        if (!reader.TryReadLittleEndian(out short length) || reader.Remaining < length)
        {
            message = default;
            return false;
        }

        reader.TryRead(out var command);

        var packetFactory = packetFactoryPool.Get(command) ?? throw new Exception($"un find {command}");

        message = packetFactory.Create();

        message.DecodeBody(ref reader, message);

        examined = consumed = input.Slice(HeaderSize + length).End;
        return true;
    }

    public void WriteMessage(MessageBase message, IBufferWriter<byte> output)
    {
        //获取头部缓冲区
        var headSpan = output.GetSpan(HeaderSize);
        output.Advance(HeaderSize);

        var keySpan = output.GetSpan(1);
        output.Advance(1);

        //写入command
        MemoryMarshal.Write(keySpan, (byte)message.Key);

        //编码
        var length = message.Encode(output) + 1;

        //写入data长度
        BinaryPrimitives.WriteInt16LittleEndian(headSpan, (short)length);
    }
}