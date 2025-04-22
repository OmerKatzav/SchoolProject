using ProtoBuf;
using Shared;

namespace Server
{
    internal class LoginService(IServerConfigService configService, IUserService userService, ICryptoService cryptoService, ICaptchaService captchaService) : ILoginService
    {
        public async Task<AuthToken> LoginAsync(string username, string password, CaptchaToken token)
        {
            if (!await captchaService.ValidateCaptchaAsync(token)) throw new ArgumentException("Invalid captcha");
            var userId = await userService.GetUserIdAsync(username);
            if (userId == Guid.Empty || !(await userService.IsValidAsync(userId, password))) throw new ArgumentException("Invalid username or password");
            var expiration = DateTime.UtcNow.Add(configService.LoginConfig.Expiration);
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, new UserData(userId, expiration));
            var signature = cryptoService.SignBytes(ms.ToArray());
            return new AuthToken(userId, expiration, signature.ToArray());
        }

        public async Task<AuthToken> RegisterAsync(string username, string password, string email, CaptchaToken token)
        {
            if (!await captchaService.ValidateCaptchaAsync(token)) throw new ArgumentException("Invalid captcha");
            var userId = await userService.AddUserAsync(username, password, email);
            var expiration = DateTime.UtcNow.Add(configService.LoginConfig.Expiration);
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, new UserData(userId, expiration));
            var signature = cryptoService.SignBytes(ms.ToArray());
            return new AuthToken(userId, expiration, signature.ToArray());
        }

        public async Task<bool> ValidateTokenAsync(AuthToken token)
        {
            using var ms = new MemoryStream();
            await Task.Run(() => Serializer.Serialize<UserData>(ms, token));
            return await Task.Run(() => cryptoService.VerifySignature(ms.ToArray(), token.Signature));
        }
    }
}
