using ProtoBuf;

namespace Shared;

[ProtoContract]
public class ChunkMetadata
{
    [ProtoMember(1)]
    public double Length { get; init; }

    [ProtoMember(2)]
    public double SecondsPerChunk { get; init; }

    [ProtoMember(3)]
    public int[]? FlatChunkSizes;

    [ProtoMember(4)] 
    public (int, int)? ChunkSizesSize;

    public int[,]? ChunkSizes
    {
        get
        {
            if (FlatChunkSizes is null || ChunkSizesSize is null) return null;
            var chunkSizes = new int[ChunkSizesSize.Value.Item1, ChunkSizesSize.Value.Item2];
            for (var i = 0; i < chunkSizes.GetLength(0); i++)
            {
                for (var j = 0; j < chunkSizes.GetLength(1); j++)
                {
                    chunkSizes[i, j] = FlatChunkSizes[i * chunkSizes.GetLength(1) + j];
                }
            }
            return chunkSizes;
        }
        init
        {
            if (value is null)
            {
                FlatChunkSizes = null;
                return;
            }
            FlatChunkSizes = new int[value.Length];
            for (var i = 0; i < value.GetLength(0); i++)
            {
                for (var j = 0; j < value.GetLength(1); j++)
                {
                    FlatChunkSizes[i * value.GetLength(1) + j] = value[i, j];
                }
            }
            ChunkSizesSize = (value.GetLength(0), value.GetLength(1));
        }
    }
}