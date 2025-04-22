using RPC;

namespace Shared
{
    [RpcService(0)]
    public interface ILoginService
    {
        [RpcMethod(0)]
        public Task<AuthToken> LoginAsync(string username, string password, CaptchaToken token);

        public Task<bool> ValidateTokenAsync(AuthToken token);

        [RpcMethod(1)]
        public Task<AuthToken> RegisterAsync(string username, string password, string email, CaptchaToken token);
    }
}
