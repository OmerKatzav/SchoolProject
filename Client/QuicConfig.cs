using System.Net.Quic;

namespace Client;

internal class QuicConfig(QuicClientConnectionOptions connectionOptions, int chunkSize)
{
    public QuicClientConnectionOptions ConnectionOptions { get; } = connectionOptions;
    public int ChunkSize { get; } = chunkSize;
}