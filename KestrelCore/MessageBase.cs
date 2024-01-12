using System.Buffers;
using MemoryPack;

namespace KestrelCore;

public abstract class MessageBase
{
    protected readonly Type Type;

    protected MessageBase(MessageKey key)
    {
        Key = key;
        Type = GetType();
    }

    /// <summary>
    /// 命令
    /// </summary>
    [MemoryPackIgnore]
    public MessageKey Key { get; set; }

    public virtual int Encode(IBufferWriter<byte> bufWriter)
    {
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Utf8);
        var writer = new MemoryPackWriter<IBufferWriter<byte>>(ref bufWriter, state);
        writer.WriteValue(Type, this);
        var writtenCount = writer.WrittenCount;
        writer.Flush();

        return writtenCount;
    }

    public virtual void DecodeBody(ref SequenceReader<byte> reader, object? context)
    {
        MemoryPackSerializer.Deserialize(Type, reader.UnreadSequence, ref context);
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, Type);
    }
}

public abstract class MessageWithIdentifier(MessageKey key) : MessageBase(key)
{
    public ulong Identifier { get; set; }
}

public abstract class RespMessageWithIdentifier(MessageKey key) : MessageWithIdentifier(key)
{
    public string? ErrorMessage { get; set; }

    public bool SuccessFul { get; set; }

    public int ErrorCode { get; set; }
}