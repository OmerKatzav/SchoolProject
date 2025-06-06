using Shared;

namespace Client;

internal interface IAbrService
{
    public Task<int> GetNewChunkIdAsync(Guid mediaId, ChunkMetadata chunkMetadata, int chunkIndex, double currentTime, double bufferSize, double lastBandwidth);

    public bool ShallAbandon(Guid mediaId, ChunkMetadata chunkMetadata, int chunkIndex, double currentTime, double bufferSize, int bitrateIndex, int bytesLeft);

    public void StopMedia(Guid mediaId);
}