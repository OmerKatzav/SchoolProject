using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class CaptchaData(byte[] captchaImage, DateTime expiration)
    {
        [ProtoMember(1)]
        public byte[] CaptchaImage { get; } = captchaImage;
        [ProtoMember(2)]
        public DateTime Expiration { get; } = expiration;
    }
}
