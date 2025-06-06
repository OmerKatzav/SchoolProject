using SixLaborsCaptcha.Core;

namespace Server;

internal class CaptchaConfig(SixLaborsCaptchaOptions captchaOptions, int length, TimeSpan expiration)
{
    public SixLaborsCaptchaOptions CaptchaOptions { get; } = captchaOptions;
    public int Length { get; } = length;
    public TimeSpan Expiration { get; } = expiration;
}