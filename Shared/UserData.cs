using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class UserData(Guid userId, DateTime expiration)
    {
        [ProtoMember(1)]
        public Guid UserId { get; } = userId;
        [ProtoMember(2)]
        public DateTime Expiration { get; } = expiration;
    }
}
