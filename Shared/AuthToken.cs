using ProtoBuf;

namespace Shared;

[ProtoContract]
public class AuthToken : AuthTokenData
{
    [ProtoMember(3)]
    public byte[]? Signature { get; init; }

    public AuthToken(AuthTokenData authTokenData, byte[] signature)
    {
        UserId = authTokenData.UserId;
        Expiration = authTokenData.Expiration;
        IssuedAt = authTokenData.IssuedAt;
        ExtraData = authTokenData.ExtraData;
        Nonce = authTokenData.Nonce;
        Purpose = authTokenData.Purpose;
        Signature = signature;
    }

    public AuthToken() { }
}