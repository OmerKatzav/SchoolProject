using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class FullCaptchaData(byte[] captchaImage, DateTime expiration, string solution) : CaptchaData(captchaImage, expiration)
    {
        [ProtoMember(3)]
        public string Solution { get; } = solution;
    }
}
