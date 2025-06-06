using ProtoBuf;
using Shared;
using System.Text;

namespace Server;

internal class LoginService(IServerConfigService configService, IUserService userService, ICryptoService cryptoService, ICaptchaService captchaService, IEmailService emailService) : ILoginService
{
    private LoginConfig LoginConfig => configService.LoginConfig ?? throw new ArgumentNullException(nameof(configService.LoginConfig));

    public async Task LoginAsync(string username, string password, CaptchaToken token)
    {
        if (!await captchaService.ValidateCaptchaAsync(token)) throw new ArgumentException("Invalid captcha");
        var userId = await userService.GetUserIdByUsernameAsync(username);
        if (userId == Guid.Empty || !await userService.IsValidAsync(userId, password)) throw new ArgumentException("Invalid username or password");
        await SendTokenAsync(GenerateNewToken(userId, AuthTokenPurpose.Login), LoginConfig.EmailLoginSubject, LoginConfig.EmailLoginBodyCreator, LoginConfig.EmailLoginHtmlBodyCreator);
    }

    public async Task RegisterAsync(string username, string password, string email, CaptchaToken token)
    {
        if (!await captchaService.ValidateCaptchaAsync(token)) throw new ArgumentException("Invalid captcha");
        if (await userService.IsUsernameTakenAsync(username)) throw new ArgumentException("Username already taken");
        if (await userService.IsEmailTakenAsync(email)) throw new ArgumentException("Email already used");
        if (!LoginConfig.UsernameRegex.IsMatch(username)) throw new ArgumentException(LoginConfig.UsernameRegexError);
        if (!LoginConfig.PasswordRegex.IsMatch(password)) throw new ArgumentException(LoginConfig.PasswordRegexError);
        if (!emailService.IsValidEmail(email)) throw new ArgumentException("Invalid Email");
        await SendTokenAsync(GenerateNewRegisterToken(username, email), LoginConfig.EmailRegisterSubject, LoginConfig.EmailRegisterBodyCreator, LoginConfig.EmailRegisterHtmlBodyCreator);
    }

    public async Task<AuthToken> FinishRegisterAsync(RegisterToken token, string password)
    {
        if (!ValidateRegisterToken(token)) throw new ArgumentException("Invalid register token");
        if (!LoginConfig.PasswordRegex.IsMatch(password)) throw new ArgumentException(LoginConfig.PasswordRegexError);
        var id = await userService.AddUserAsync(token.Username, password, token.Email, await userService.GetUserCountAsync() == 0);
        return GenerateNewToken(id, AuthTokenPurpose.FullAccess, Encoding.UTF8.GetBytes(token.Email));
    }

    public async Task ForgotPasswordAsync(string email, string newPassword, CaptchaToken token)
    {
        if (!await captchaService.ValidateCaptchaAsync(token)) throw new ArgumentException("Invalid captcha");
        if (!LoginConfig.PasswordRegex.IsMatch(newPassword)) throw new ArgumentException(LoginConfig.PasswordRegexError);
        Guid userId;
        try
        {
            userId = await userService.GetUserIdByEmailAsync(email);
        }
        catch (InvalidOperationException)
        {
            return;
        }
        await SendTokenAsync(GenerateNewToken(userId, AuthTokenPurpose.AccountRecovery), LoginConfig.EmailForgotPasswordSubject, LoginConfig.EmailForgotPasswordBodyCreator, LoginConfig.EmailForgotPasswordHtmlBodyCreator);
    }

    public async Task<bool> ValidateTokenAsync(AuthToken token)
    {
        var signature = token.Signature!;
        var expiration = token.Expiration;
        var userId = token.UserId;
        using var ms = new MemoryStream();
        Serializer.Serialize<AuthTokenData>(ms, token);
        return await userService.IsUserIdTakenAsync(userId) && DateTime.UtcNow < expiration && token.IssuedAt > await userService.GetPasswordLastChangedAsync(userId) && cryptoService.VerifySignature(ms.ToArray(), signature);
    }

    private bool ValidateRegisterToken(RegisterToken token)
    {
        var signature = token.Signature!;
        var expiration = token.Expiration;
        using var ms = new MemoryStream();
        Serializer.Serialize<RegisterData>(ms, token);
        return DateTime.UtcNow < expiration && cryptoService.VerifySignature(ms.ToArray(), signature);
    }

    public async Task ChangePasswordAsync(AuthToken authToken, string oldPassword, string newPassword, CaptchaToken captchaToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.FullAccess) throw new ArgumentException("Invalid auth token");
        if (!await captchaService.ValidateCaptchaAsync(captchaToken)) throw new ArgumentException("Invalid captcha");
        var userId = authToken.UserId;
        if (!await userService.IsValidAsync(userId, oldPassword)) throw new ArgumentException("Invalid password");
        if (!LoginConfig.PasswordRegex.IsMatch(newPassword)) throw new ArgumentException(LoginConfig.PasswordRegexError);
        await SendTokenAsync(GenerateNewToken(userId, AuthTokenPurpose.AccountRecovery), LoginConfig.EmailChangePasswordSubject, LoginConfig.EmailChangePasswordBodyCreator, LoginConfig.EmailChangePasswordHtmlBodyCreator);
    }

    public async Task FinishForgotPasswordAsync(AuthToken authToken, string newPassword)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.AccountRecovery) throw new ArgumentException("Invalid auth token");
        if (!LoginConfig.PasswordRegex.IsMatch(newPassword)) throw new ArgumentException(LoginConfig.PasswordRegexError);
        var userId = authToken.UserId;
        await userService.SetPasswordAsync(userId, newPassword);
    }

    public async Task ChangeEmailAsync(AuthToken authToken, string newEmail, CaptchaToken captchaToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.FullAccess) throw new ArgumentException("Invalid auth token");
        if (!await captchaService.ValidateCaptchaAsync(captchaToken)) throw new ArgumentException("Invalid captcha");
        if (await userService.IsEmailTakenAsync(newEmail)) throw new ArgumentException("Email already taken");
        if (!emailService.IsValidEmail(newEmail)) throw new ArgumentException("Invalid Email");
        var userId = authToken.UserId;
        await SendTokenAsync(GenerateNewToken(userId, AuthTokenPurpose.ChangeEmail, Encoding.UTF8.GetBytes(newEmail)), LoginConfig.EmailChangeEmailSubject, LoginConfig.EmailChangeEmailBodyCreator, LoginConfig.EmailChangeEmailHtmlBodyCreator);
    }

    public async Task FinishChangeEmailAsync(AuthToken authToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.ChangeEmail) throw new ArgumentException("Invalid auth token");
        var userId = authToken.UserId;
        var newEmail = authToken.ExtraData ?? throw new ArgumentNullException(nameof(authToken));
        await userService.SetEmailAsync(userId, Encoding.UTF8.GetString(newEmail));
    }

    public async Task ChangeUsernameAsync(AuthToken authToken, string newUsername, CaptchaToken captchaToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.FullAccess) throw new ArgumentException("Invalid auth token");
        if (!await captchaService.ValidateCaptchaAsync(captchaToken)) throw new ArgumentException("Invalid captcha");
        if (await userService.IsUsernameTakenAsync(newUsername)) throw new ArgumentException("Username already taken");
        if (!LoginConfig.UsernameRegex.IsMatch(newUsername)) throw new ArgumentException(LoginConfig.UsernameRegexError);
        var userId = authToken.UserId;
        await userService.SetUsernameAsync(userId, newUsername);
    }

    public async Task<string> GetUsernameAsync(Guid userId)
    {
        if (!await userService.IsUserIdTakenAsync(userId)) throw new ArgumentException("Invalid user id");
        return await userService.GetUsernameAsync(userId);
    }

    public async Task<AuthToken> RefreshTokenAsync(AuthToken authToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not (AuthTokenPurpose.FullAccess or AuthTokenPurpose.Login)) throw new ArgumentException("Invalid auth token");
        var userId = authToken.UserId;
        return GenerateNewToken(userId);
    }

    private RegisterToken GenerateNewRegisterToken(string username, string email)
    {
        var expiration = DateTime.UtcNow.Add(LoginConfig.TemporaryExpiration);
        var registerData = new RegisterData { Username = username, Email = email, Expiration = expiration, Nonce = [.. cryptoService.GenerateSalt()] };
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, registerData);
        var signature = cryptoService.SignBytes(ms.ToArray()).ToArray();
        return new RegisterToken(registerData, signature);
    }

    private AuthToken GenerateNewToken(Guid userId, AuthTokenPurpose purpose = AuthTokenPurpose.FullAccess, byte[]? extraData = null)
    {
        var expiration = DateTime.UtcNow.Add(purpose == AuthTokenPurpose.FullAccess ? LoginConfig.Expiration : LoginConfig.TemporaryExpiration);
        var authTokenData = new AuthTokenData
        { UserId = userId, Expiration = expiration, Purpose = purpose, ExtraData = extraData, Nonce = [.. cryptoService.GenerateSalt()] };
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, authTokenData);
        var signature = cryptoService.SignBytes(ms.ToArray()).ToArray();
        return new AuthToken(authTokenData, signature);
    }

    private async Task SendTokenAsync(AuthToken token, string emailSubject, Func<string, string, string> emailBodyCreator, Func<string, string, string>? emailHtmlBodyCreator)
    {
        var userId = token.UserId;
        var email = await userService.GetEmailAsync(userId);
        var username = await userService.GetUsernameAsync(userId);
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, token);
        var base64Code = Convert.ToBase64String(ms.ToArray());
        await emailService.SendEmailAsync(email, emailSubject, emailBodyCreator(username, base64Code), emailHtmlBodyCreator?.Invoke(username, base64Code));
    }

    private async Task SendTokenAsync(RegisterToken token, string emailSubject, Func<string, string, string> emailBodyCreator, Func<string, string, string>? emailHtmlBodyCreator)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, token);
        var base64Code = Convert.ToBase64String(ms.ToArray());
        var email = token.Email;
        var username = token.Username;
        await emailService.SendEmailAsync(email, emailSubject, emailBodyCreator(username, base64Code), emailHtmlBodyCreator?.Invoke(username, base64Code));
    }

    public async Task<bool> IsAdminAsync(AuthToken authToken)
    {
        if (!await ValidateTokenAsync(authToken) || authToken.Purpose is not AuthTokenPurpose.FullAccess) throw new ArgumentException("Invalid auth token");
        return await userService.IsAdminAsync(authToken.UserId);
    }
}