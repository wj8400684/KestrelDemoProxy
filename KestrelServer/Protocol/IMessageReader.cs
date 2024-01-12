using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace KestrelCore;

public interface IMessageReader<TMessage>
{
    bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined,
        [MaybeNullWhen(false)] out TMessage message);
}