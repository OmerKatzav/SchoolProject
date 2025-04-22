using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class AuthToken(Guid userId, DateTime expiration, byte[] signature) : UserData(userId, expiration)
    {
        [ProtoMember(3)]
        public byte[] Signature { get; } = signature;
    }
}
