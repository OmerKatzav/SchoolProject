using RPC;

namespace Shared;

[RpcService(1)]
public interface ICaptchaService
{
    [RpcMethod(0)]
    public Task<ValueTuple<CaptchaData, byte[]>> GetCaptchaAsync();

    [RpcMethod(1)]
    public Task<bool> ValidateCaptchaAsync(CaptchaToken token);
}