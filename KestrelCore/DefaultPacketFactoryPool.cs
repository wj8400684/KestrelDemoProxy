namespace KestrelCore;

public interface IPacketFactoryPool
{
    IMessageFactory Get(MessageKey command);

    IMessageFactory? Get(byte command);
}

public class DefaultPacketFactoryPool : IPacketFactoryPool
{
    private IMessageFactory[] _packetFactories;

    public DefaultPacketFactoryPool()
    {
        Inilizetion();
    }

    private void Inilizetion()
    {
        _packetFactories = new IMessageFactory[10];

        RegisterPacketType<LoginRequest>(MessageKey.Login);
        RegisterPacketType<LoginResponse>(MessageKey.LoginAck);
    }

    private void RegisterPacketType<TMessage>(MessageKey packetType)
        where TMessage : MessageBase, new()
    {
        _packetFactories[(int)packetType] = new DefaultMessageFactory<TMessage>();
    }

    public IMessageFactory Get(MessageKey command)
    {
        return Get((byte)command)!;
    }

    public IMessageFactory? Get(byte command)
    {
        if (command > _packetFactories.Length)
            return null;

        return _packetFactories[command];
    }
}