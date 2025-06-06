namespace Client;

internal class BolaConfig(double maxBufferSize, double minBufferSize)
{
    public double MaxBufferSize { get; } = maxBufferSize;
    public double MinBufferSize { get; } = minBufferSize;
}