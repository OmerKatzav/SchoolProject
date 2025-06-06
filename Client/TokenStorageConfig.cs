namespace Client;

internal class TokenStorageConfig(Stream storageStream)
{
    public Stream StorageStream { get; } = storageStream;
}