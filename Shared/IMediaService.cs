using RPC;

namespace Shared;

[RpcService(2)]
public interface IMediaService
{
    [RpcMethod(0)]
    public Task<Guid[]> GetMediaIdsAsync(AuthToken authToken);

    [RpcMethod(1)]
    public Task<Guid[]> SearchMediasAsync(AuthToken authToken, string searchTerm);

    [RpcMethod(2)]
    public Task<string> GetMediaNameAsync(AuthToken authToken, Guid mediaId);

    [RpcMethod(3)]
    public Task<byte[]> GetMediaThumbnailAsync(AuthToken authToken, Guid mediaId);

    [RpcMethod(4)]
    public Task<ChunkMetadata> GetChunkMetadataAsync(AuthToken authToken, Guid mediaId);

    [RpcMethod(5)]
    public Task<byte[]> GetChunkAsync(AuthToken authToken, Guid mediaId, int chunkIndex, int chunkBitrateIndex, Action<CallMetadata>? downloadCallback = null, CancellationToken ctx = default);

    [RpcMethod(6)]
    public Task<Guid> InsertMediaAsync(AuthToken authToken, string name, byte[] thumbnail, byte[] media);

    [RpcMethod(7)]
    public Task RemoveMediaAsync(AuthToken authToken, Guid mediaId);
}