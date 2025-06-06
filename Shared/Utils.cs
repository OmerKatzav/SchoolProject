namespace Shared;

public static class Utils
{
    public static async Task<byte[]> ReadBytesAsync(Stream stream, int length, int chunkSize = int.MaxValue, CancellationToken cancellationToken = default, Action<int>? readBytesCallback = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var buffer = new byte[length];
        var bytesRead = 0;
        while (bytesRead < length)
        {
            var newBytes = await stream.ReadAsync(new Memory<byte>(buffer, bytesRead, Math.Min(length - bytesRead, chunkSize)), cancellationToken);
            if (newBytes == 0)
            {
                throw new EndOfStreamException($"Stream closed with {length - bytesRead} still expected");
            }
            readBytesCallback?.Invoke(newBytes);
            bytesRead += newBytes;
        }
        return buffer;
    }
}
