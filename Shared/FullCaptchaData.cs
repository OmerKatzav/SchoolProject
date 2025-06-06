using ProtoBuf;

namespace Shared;

[ProtoContract]
public class FullCaptchaData : CaptchaData
{
    [ProtoMember(4)]
    public string? Solution { get; init; }

    public FullCaptchaData(CaptchaData captchaData, string? solution)
    {
        Solution = solution;
        CaptchaImage = captchaData.CaptchaImage;
        Expiration = captchaData.Expiration;
        Nonce = captchaData.Nonce;
    }
    
    public FullCaptchaData() {}
}