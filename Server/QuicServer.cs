using Microsoft.Extensions.Logging;
using RPC;
using Shared;
using System.Net.Quic;

namespace Server;

internal class QuicServer(IServerConfigService configService, IRequestHandler requestHandler, ILogger logger) : IServer
{
    private QuicListener? _listener;
    private bool _isRunning;
    private QuicConfig QuicConfig => configService.QuicConfig ?? throw new ArgumentNullException(nameof(configService.QuicConfig));

    public async Task StartAsync()
    {
        if (!QuicListener.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
        _listener = await QuicListener.ListenAsync(QuicConfig.ListenerOptions);
        logger.LogInformation("Listening on {LocalEndPoint}", _listener.LocalEndPoint);
        _isRunning = true;
        while (_isRunning)
        {
            var connection = await _listener.AcceptConnectionAsync();
            logger.LogInformation("Accepted connection from {RemoteEndPoint}", connection.RemoteEndPoint);
            _ = HandleConnectionAsync(connection);
        }
    }

    private async Task HandleConnectionAsync(QuicConnection connection)
    {
        if (!QuicConnection.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
        try
        {
            while (_isRunning)
            {
                var stream = await connection.AcceptInboundStreamAsync();
                logger.LogInformation("Accepted stream from {RemoteEndPoint}", connection.RemoteEndPoint);
                _ = HandleStreamAsync(stream);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling connection");
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    private async Task HandleStreamAsync(QuicStream stream)
    {
        if (!QuicConnection.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
        try
        {
            var lengthBytes = await Utils.ReadBytesAsync(stream, 4);
            var length = BitConverter.ToInt32(lengthBytes);
            if (length <= 0) throw new ArgumentException("Invalid length");
            var request = await Utils.ReadBytesAsync(stream, length);
            var response = requestHandler.HandleRequest(request).ToArray();
            var responseLength = BitConverter.GetBytes(response.Length);
            await stream.WriteAsync(new ReadOnlyMemory<byte>(responseLength));
            await stream.WriteAsync(new ReadOnlyMemory<byte>(response));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling connection");
        }
        finally
        {
            await stream.DisposeAsync();
        }
    }

    public async Task StopAsync()
    {
        if (!QuicListener.IsSupported) throw new NotSupportedException("Quic is not supported on this platform");
        _isRunning = false;
        if (_listener == null) return;
        await _listener.DisposeAsync();
    }
}