using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class MediaInfo(Guid id, byte[] image, string name)
    {
        [ProtoMember(1)]
        public Guid Id { get; } = id;
        [ProtoMember(2)]
        public byte[] Image { get; } = image;
        [ProtoMember(3)]
        public string Name { get; } = name;
    }
}
