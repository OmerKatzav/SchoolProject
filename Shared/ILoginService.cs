using RPC;

namespace Shared;

[RpcService(0)]
public interface ILoginService
{
    [RpcMethod(0)]
    public Task LoginAsync(string username, string password, CaptchaToken token);

    [RpcMethod(1)]
    public Task<bool> ValidateTokenAsync(AuthToken token);

    [RpcMethod(2)]
    public Task RegisterAsync(string username, string password, string email, CaptchaToken token);

    [RpcMethod(3)]
    public Task<AuthToken> FinishRegisterAsync(RegisterToken token, string password);

    [RpcMethod(3)]
    public Task ForgotPasswordAsync(string email, string newPassword, CaptchaToken token);

    [RpcMethod(4)]
    public Task ChangePasswordAsync(AuthToken authToken, string oldPassword, string newPassword, CaptchaToken captchaToken);

    [RpcMethod(5)]
    public Task FinishForgotPasswordAsync(AuthToken authToken, string newPassword);

    [RpcMethod(6)]
    public Task ChangeEmailAsync(AuthToken authToken, string newEmail, CaptchaToken captchaToken);

    [RpcMethod(7)]
    public Task FinishChangeEmailAsync(AuthToken authToken);

    [RpcMethod(8)]
    public Task ChangeUsernameAsync(AuthToken authToken, string newUsername, CaptchaToken captchaToken);

    [RpcMethod(9)]
    public Task<string> GetUsernameAsync(Guid userId);

    [RpcMethod(10)]
    public Task<AuthToken> RefreshTokenAsync(AuthToken authToken);

    [RpcMethod(11)]
    public Task<bool> IsAdminAsync(AuthToken authToken);
}