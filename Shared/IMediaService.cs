using RPC;

namespace Shared
{
    [RpcService(2)]
    public interface IMediaService
    {
        [RpcMethod(0)]
        public Task<MediaInfo[]> GetMediaInfosAsync(AuthToken authToken);

        [RpcMethod(1)]
        public Task<ChunkMetadata> GetChunkMetadataAsync(AuthToken authToken, Guid mediaId);

        [RpcMethod(2)]
        public Task<byte[]> GetChunk(AuthToken authToken, Guid mediaId, int chunkIndex, int chunkBitrateIndex); 
    }
}
