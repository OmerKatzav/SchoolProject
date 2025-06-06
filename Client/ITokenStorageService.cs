using Shared;

namespace Client;

internal interface ITokenStorageService
{
    public AuthToken? AuthToken { get; set; }
    public CaptchaToken? CaptchaToken { get; set; }
}