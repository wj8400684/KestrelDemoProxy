using System.Buffers;

namespace KestrelCore;

public interface IMessageWriter<in TMessage>
{
    void WriteMessage(TMessage message, IBufferWriter<byte> output);
}