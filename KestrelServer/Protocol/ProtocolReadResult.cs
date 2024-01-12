namespace KestrelCore;

public readonly struct ProtocolReadResult<TMessage>(TMessage? message, bool isCanceled, bool isCompleted)
{
    public TMessage? Message { get; } = message;
    public bool IsCanceled { get; } = isCanceled;
    public bool IsCompleted { get; } = isCompleted;
}