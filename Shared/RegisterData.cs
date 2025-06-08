using ProtoBuf;

namespace Shared;

[ProtoInclude(10, typeof(RegisterToken))]
[ProtoContract]
public class RegisterData
{
    [ProtoMember(1)]
    public string Username { get; init; } = null!;

    [ProtoMember(2)]
    public string Email { get; init; } = null!;

    [ProtoMember(3)]
    public DateTime Expiration { get; init; }

    [ProtoMember(4)]
    public byte[]? Nonce { get; init; }
}