using System.Security.Cryptography;

namespace Server;

internal class CryptoConfig(ECDsa signatureKey, HashAlgorithmName signatureHashAlgorithm, int passwordHashSize, int passwordSaltSize, int argonDegreesOfParallelism, int argonMemorySize, int argonIterations)
{
    public ECDsa SignatureKey { get; } = signatureKey;
    public HashAlgorithmName SignatureHashAlgorithm { get; } = signatureHashAlgorithm;
    public int PasswordHashSize { get; } = passwordHashSize;
    public int PasswordSaltSize { get; } = passwordSaltSize;
    public int ArgonDegreeOfParallelism { get; } = argonDegreesOfParallelism;
    public int ArgonMemorySize { get; } = argonMemorySize;
    public int ArgonIterations { get; } = argonIterations;
}