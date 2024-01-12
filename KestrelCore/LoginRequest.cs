using System.Buffers;
using MemoryPack;

namespace KestrelCore;

[MemoryPackable]
public sealed partial class LoginRequest() : MessageBase(MessageKey.Login)
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}

[MemoryPackable]
public sealed partial class LoginResponse() : RespMessageWithIdentifier(MessageKey.LoginAck);