using ProtoBuf;
using Shared;

namespace Client;

internal class TokenStorageService(IClientConfigService configService) : ITokenStorageService
{
    private TokenStorageConfig TokenStorageConfig => configService.TokenStorageConfig ?? throw new ArgumentNullException(nameof(configService.TokenStorageConfig));

    public AuthToken? AuthToken
    {
        get
        {
            TokenStorageConfig.StorageStream.Position = 0;
            return Serializer.Deserialize<ValueTuple<AuthToken?, CaptchaToken?>>(TokenStorageConfig.StorageStream).Item1;
        }
        set
        {
            var captchaToken = CaptchaToken;
            TokenStorageConfig.StorageStream.SetLength(0);
            TokenStorageConfig.StorageStream.Position = 0;
            Serializer.Serialize(TokenStorageConfig.StorageStream, new ValueTuple<AuthToken?, CaptchaToken?>(value, captchaToken));
            TokenStorageConfig.StorageStream.SetLength(TokenStorageConfig.StorageStream.Position);
        }
    }

    public CaptchaToken? CaptchaToken
    {
        get
        {
            TokenStorageConfig.StorageStream.Position = 0;
            return Serializer.Deserialize<ValueTuple<AuthToken?, CaptchaToken?>>(TokenStorageConfig.StorageStream).Item2;
        }
        set
        {
            var authToken = AuthToken;
            TokenStorageConfig.StorageStream.SetLength(0);
            TokenStorageConfig.StorageStream.Position = 0;
            Serializer.Serialize(TokenStorageConfig.StorageStream, new ValueTuple<AuthToken?, CaptchaToken?>(authToken, value));
            TokenStorageConfig.StorageStream.SetLength(TokenStorageConfig.StorageStream.Position);
        }
    }
}