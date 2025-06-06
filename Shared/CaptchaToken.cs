using ProtoBuf;

namespace Shared;

[ProtoContract]
public class CaptchaToken : FullCaptchaData
{
    [ProtoMember(5)]
    public byte[]? Signature { get; init; }

    public CaptchaToken(FullCaptchaData fullCaptchaData, byte[] signature)
    {
        Signature = signature;
        Solution = fullCaptchaData.Solution;
        CaptchaImage = fullCaptchaData.CaptchaImage;
        Expiration = fullCaptchaData.Expiration;
        Nonce = fullCaptchaData.Nonce;
    }

    public CaptchaToken() { }
}