using ProtoBuf;

namespace Shared;

[ProtoContract]
[ProtoInclude(10, typeof(AuthToken))]
public class AuthTokenData
{
    [ProtoMember(1)]
    public Guid UserId { get; init; }

    [ProtoMember(2)]
    public DateTime Expiration { get; init; }

    [ProtoMember(3)]
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;

    [ProtoMember(4)]
    public byte[]? ExtraData { get; init; }

    [ProtoMember(5)]
    public byte[]? Nonce { get; init; }

    [ProtoMember(6)]
    public AuthTokenPurpose Purpose { get; init; } = AuthTokenPurpose.FullAccess;
}