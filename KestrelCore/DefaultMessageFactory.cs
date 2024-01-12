namespace KestrelCore;


public interface IMessageFactory
{
    MessageBase Create();

    void Return(MessageBase package);
}

public sealed class DefaultMessageFactory<TPacket> : IMessageFactory
    where TPacket : MessageBase, new()
{
    public MessageBase Create()
    {
        return new TPacket();
    }

    public void Return(MessageBase package)
    {
    }
}