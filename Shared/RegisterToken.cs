using ProtoBuf;

namespace Shared;

[ProtoContract]
public class RegisterToken : RegisterData
{
    [ProtoMember(5)]
    public byte[]? Signature { get; init; }

    public RegisterToken(RegisterData registerData, byte[] signature)
    {
        Username = registerData.Username;
        Email = registerData.Email;
        Expiration = registerData.Expiration;
        Nonce = registerData.Nonce;
        Signature = signature;
    }

    public RegisterToken() { }
}