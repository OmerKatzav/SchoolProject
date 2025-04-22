using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class CaptchaToken(byte[] captchaImage, DateTime expiration, string solution, byte[] signature) : FullCaptchaData(captchaImage, expiration, solution)
    {
        [ProtoMember(4)]
        public byte[] Signature { get; } = signature;
    }
}
