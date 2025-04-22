namespace Shared
{
    public static class Utils
    {
        public static async Task<byte[]> ReadBytesAsync(Stream stream, int length)
        {
            var buffer = new byte[length];
            var bytesRead = 0;
            while (bytesRead < length)
            {

                var newBytes = await stream.ReadAsync(new Memory<byte>(buffer, bytesRead, length - bytesRead));
                if (newBytes == 0)
                {
                    throw new EndOfStreamException($"Stream closed with {length - bytesRead} still expected");
                }
                bytesRead += newBytes;
            }
            return buffer;
        }
    }
}
