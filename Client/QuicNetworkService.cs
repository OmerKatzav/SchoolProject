using Microsoft.Extensions.Logging;
using RPC;
using Shared;
using System.Diagnostics;
using System.Net.Quic;

namespace Client;

internal class QuicNetworkService(IClientConfigService configService, ILogger logger) : INetworkService
{
    private QuicConnection? _connection;
    private QuicConfig QuicConfig => configService.QuicConfig ?? throw new ArgumentNullException(nameof(configService.QuicConfig));

    public async Task StartAsync()
    {
        if (!QuicConnection.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
        _connection = await QuicConnection.ConnectAsync(QuicConfig.ConnectionOptions);
        logger.LogInformation("Connected to {RemoteEndPoint}", _connection.RemoteEndPoint);
    }

    public async Task<IEnumerable<byte>> RequestAsync(IEnumerable<byte> data, Action<CallMetadata>? callback = null, CancellationToken ctx = default)
    {
        if (_connection == null) throw new InvalidOperationException("Connection is not established");
        var stream = await _connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional, ctx);
        logger.LogInformation("Opened stream to {RemoteEndPoint}", _connection.RemoteEndPoint);
        var metadata = new CallMetadata
        {
            ResponseBytesRead = 0,
            ResponseBandwidth = 0
        };
        try
        {
            ctx.ThrowIfCancellationRequested();
            var dataArr = data.ToArray();
            var length = BitConverter.GetBytes(dataArr.Length);
            await stream.WriteAsync(new ReadOnlyMemory<byte>(length), ctx);
            await stream.WriteAsync(new ReadOnlyMemory<byte>(dataArr), ctx);
            var responseLengthBytes = await Utils.ReadBytesAsync(stream, 4, QuicConfig.ChunkSize, ctx);
            var responseLength = BitConverter.ToInt32(responseLengthBytes);
            metadata.ResponseLength = responseLength + 4;
            metadata.ResponseBytesRead = 4;
            if (responseLength <= 0) throw new ArgumentException("Invalid length");
            var sw = Stopwatch.StartNew();
            return await Utils.ReadBytesAsync(stream, responseLength, QuicConfig.ChunkSize, ctx, bytesRead =>
            {
                metadata.ResponseBytesRead += bytesRead;
                metadata.ResponseBandwidth = metadata.ResponseBytesRead / sw.Elapsed.TotalSeconds;
                callback?.Invoke(metadata);
            });
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }

    public IEnumerable<byte> Request(IEnumerable<byte> data, Action<CallMetadata>? callback = null, CancellationToken ctx = default)
    {
        return RequestAsync(data, callback, ctx).Result;
    }
}