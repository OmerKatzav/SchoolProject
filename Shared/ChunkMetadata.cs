namespace Shared
{
    public class ChunkMetadata(double length, double secondsPerChunk, double[] chunkBitrates) 
    {
        public double Length { get; } = length;
        public double SecondsPerChunk { get; } = secondsPerChunk;
        public double[] ChunkBitrates { get; } = chunkBitrates;
    }
}
