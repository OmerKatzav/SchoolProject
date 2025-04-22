using Shared;

namespace Client
{
    internal interface IAbrService
    {
        public Task<byte[]> GetNewChunkAsync(Guid mediaId, ChunkMetadata chunkMetadata, int chunkIndex, double currentTime, AuthToken token);
    }
}
