namespace Shared;

public class ChunkMetadata
{
    public double Length { get; init; }
    public double SecondsPerChunk { get; init; }
    public int[,]? ChunkSizes { get; init; }
}