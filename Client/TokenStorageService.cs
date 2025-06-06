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
            TokenStorageConfig.StorageStream.Position = 0;
            Serializer.Serialize(TokenStorageConfig.StorageStream, new ValueTuple<AuthToken?, CaptchaToken?>(value, CaptchaToken));
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
            TokenStorageConfig.StorageStream.Position = 0;
            Serializer.Serialize(TokenStorageConfig.StorageStream, new ValueTuple<AuthToken?, CaptchaToken?>(AuthToken, value));
            TokenStorageConfig.StorageStream.SetLength(TokenStorageConfig.StorageStream.Position);
        }
    }
}