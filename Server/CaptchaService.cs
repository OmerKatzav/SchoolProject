using ProtoBuf;
using Shared;
using SixLaborsCaptcha.Core;

namespace Server
{
    internal class CaptchaService(ICryptoService cryptoService, IServerConfigService configService) : ICaptchaService
    {
        public Task<(CaptchaData, byte[])> GetCaptchaAsync()
        {
            return Task.Run(() =>
            {
                var slc = new SixLaborsCaptchaModule(configService.CaptchaConfig.CaptchaOptions);
                var answer = Extensions.GetUniqueKey(configService.CaptchaConfig.Length);
                var captcha = slc.Generate(answer);
                var expiration = DateTime.UtcNow.Add(configService.CaptchaConfig.Expiration);
                var captchaData = new FullCaptchaData(captcha, expiration, answer);
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
                using var ms = new MemoryStream();
                Serializer.Serialize<FullCaptchaData>(ms, token);
                return cryptoService.VerifySignature(ms.ToArray(), token.Signature);
            });
        }
    }
}
