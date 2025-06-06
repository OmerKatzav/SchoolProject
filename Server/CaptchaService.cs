using ProtoBuf;
using Shared;
using SixLaborsCaptcha.Core;

namespace Server;

internal class CaptchaService(ICryptoService cryptoService, IServerConfigService configService) : ICaptchaService
{
    private CaptchaConfig CaptchaConfig => configService.CaptchaConfig ?? throw new ArgumentNullException(nameof(configService.CaptchaConfig));

    public Task<ValueTuple<CaptchaData, byte[]>> GetCaptchaAsync()
    {
        return Task.Run(() =>
        {
            var slc = new SixLaborsCaptchaModule(CaptchaConfig.CaptchaOptions);
            var solution = Extensions.GetUniqueKey(CaptchaConfig.Length);
            var captcha = slc.Generate(solution);
            var expiration = DateTime.UtcNow.Add(CaptchaConfig.Expiration);
            var captchaData = new FullCaptchaData { CaptchaImage = captcha, Expiration = expiration, Solution = solution, Nonce = [.. cryptoService.GenerateSalt()] };
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, captchaData);
            var signature = cryptoService.SignBytes(ms.ToArray());
            return ((CaptchaData)captchaData, signature.ToArray());
        });
    }

    public Task<bool> ValidateCaptchaAsync(CaptchaToken token)
    {
        return Task.Run(() =>
        {
            var signature = token.Signature ?? throw new ArgumentNullException(nameof(token));
            var expiration = token.Expiration;
            if (token.CaptchaImage is null || token.Solution is null) throw new ArgumentNullException(nameof(token));
            using var ms = new MemoryStream();
            Serializer.Serialize<FullCaptchaData>(ms, token);
            return DateTime.UtcNow < expiration && cryptoService.VerifySignature(ms.ToArray(), signature);
        });
    }
}