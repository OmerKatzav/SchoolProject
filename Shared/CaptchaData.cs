using ProtoBuf;

namespace Shared;

[ProtoContract]
[ProtoInclude(10, typeof(FullCaptchaData))]
[ProtoInclude(20, typeof(CaptchaToken))]
public class CaptchaData
{
    [ProtoMember(1)]
    public byte[] CaptchaImage { get; init; } = null!;

    [ProtoMember(2)]
    public DateTime Expiration { get; init; }

    [ProtoMember(3)]
    public byte[]? Nonce { get; init; }
}