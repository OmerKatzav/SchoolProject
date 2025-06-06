using Konscious.Security.Cryptography;
using System.Security.Cryptography;

namespace Server;

internal class CryptoService(IServerConfigService configService) : ICryptoService
{
    private CryptoConfig CryptoConfig => configService.CryptoConfig ?? throw new ArgumentNullException(nameof(configService.CryptoConfig));

    public IEnumerable<byte> SignBytes(IEnumerable<byte> data)
    {
        return CryptoConfig.SignatureKey.SignData([.. data], CryptoConfig.SignatureHashAlgorithm);
    }

    public bool VerifySignature(IEnumerable<byte> data, IEnumerable<byte> signature)
    {
        return CryptoConfig.SignatureKey.VerifyData(data.ToArray(), signature.ToArray(), CryptoConfig.SignatureHashAlgorithm);
    }

    public IEnumerable<byte> HashBytes(IEnumerable<byte> data, IEnumerable<byte> salt,
        IEnumerable<byte> associatedData)
    {
        var argon = new Argon2id([.. data])
        {
            AssociatedData = [.. associatedData],
            Salt = [.. salt],
        };
        return argon.GetBytes(CryptoConfig.PasswordHashSize);
    }

    public IEnumerable<byte> GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(CryptoConfig.PasswordSaltSize);
    }
}