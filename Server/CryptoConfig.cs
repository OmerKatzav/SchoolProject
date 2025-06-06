using System.Security.Cryptography;

namespace Server;

internal class CryptoConfig(ECDsa signatureKey, HashAlgorithmName signatureHashAlgorithm, int passwordHashSize, int passwordSaltSize)
{
    public ECDsa SignatureKey { get; } = signatureKey;
    public HashAlgorithmName SignatureHashAlgorithm { get; } = signatureHashAlgorithm;
    public int PasswordHashSize { get; } = passwordHashSize;
    public int PasswordSaltSize { get; } = passwordSaltSize;
}