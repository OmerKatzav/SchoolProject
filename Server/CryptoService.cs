using Konscious.Security.Cryptography;
using System.Security.Cryptography;

namespace Server
{
    internal class CryptoService(IServerConfigService configService) : ICryptoService
    {
        public IEnumerable<byte> SignBytes(IEnumerable<byte> data)
        {
            return configService.CryptoConfig.SignatureKey.SignData([.. data], configService.CryptoConfig.SignatureHashAlgorithm);
        }

        public bool VerifySignature(IEnumerable<byte> data, IEnumerable<byte> signature)
        {
            return configService.CryptoConfig.SignatureKey.VerifyData(data.ToArray(), signature.ToArray(), configService.CryptoConfig.SignatureHashAlgorithm);
        }

        public IEnumerable<byte> HashBytes(IEnumerable<byte> data, IEnumerable<byte> salt,
            IEnumerable<byte> associatedData)
        {
            var argon = new Argon2id([.. data])
            {
                AssociatedData = [.. associatedData],
                Salt = [.. salt],
            };
            return argon.GetBytes(configService.CryptoConfig.PasswordHashSize);
        }

        public IEnumerable<byte> GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(configService.CryptoConfig.PasswordSaltSize);
        }
    }
}
