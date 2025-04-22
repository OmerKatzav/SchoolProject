using System.Net.Quic;

namespace Client
{
    internal class QuicConfig(QuicClientConnectionOptions connectionOptions)
    {
        public QuicClientConnectionOptions ConnectionOptions { get; } = connectionOptions;
    }
}
