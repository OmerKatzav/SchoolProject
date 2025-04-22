using System.Net.Quic;

namespace Server
{
    internal class QuicConfig(QuicListenerOptions listenerOptions)
    {
        public QuicListenerOptions ListenerOptions { get; } = listenerOptions;
    }
}
