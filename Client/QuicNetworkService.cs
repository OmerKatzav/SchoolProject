using Microsoft.Extensions.Logging;
using RPC;
using Shared;
using System.Net.Quic;

namespace Client
{
    internal class QuicNetworkService(QuicClientConnectionOptions options, ILogger logger) : INetworkService
    {
        private QuicConnection? _connection;

        public async Task StartAsync()
        {
            if (!QuicConnection.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
            _connection = await QuicConnection.ConnectAsync(options);
            logger.LogInformation("Connected to {RemoteEndPoint}", _connection.RemoteEndPoint);
        }

        public async Task<IEnumerable<byte>> RequestAsync(IEnumerable<byte> data)
        {
            if (_connection == null) throw new InvalidOperationException("Connection is not established");
            var stream = await _connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
            logger.LogInformation("Opened stream to {RemoteEndPoint}", _connection.RemoteEndPoint);
            try
            {
                var dataArr = data.ToArray();
                var length = BitConverter.GetBytes(dataArr.Length);
                await stream.WriteAsync(new ReadOnlyMemory<byte>(length));
                await stream.WriteAsync(new ReadOnlyMemory<byte>(dataArr));
                var responseLengthBytes = await Utils.ReadBytesAsync(stream, 4);
                var responseLength = BitConverter.ToInt32(responseLengthBytes);
                if (responseLength <= 0) throw new ArgumentException("Invalid length");
                var response = await Utils.ReadBytesAsync(stream, responseLength);
                return response;
            }
            finally
            {
                await stream.DisposeAsync();
            }
        }

        public IEnumerable<byte> Request(IEnumerable<byte> data)
        {
            return RequestAsync(data).Result;
        }
    }
}
