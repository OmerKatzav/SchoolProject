using Shared;

namespace Server;

internal interface IMediaInternalService
{
    public IEnumerable<Guid> GetMediaIds();
    public IEnumerable<Guid> SearchMedia(string searchTerm);
    public Task<string> GetMediaNameAsync(Guid mediaId);
    public Task<IEnumerable<byte>> GetMediaThumbnailAsync(Guid mediaId);
    public Task<ChunkMetadata> GetChunkMetadataAsync(Guid mediaId);
    public Task<IEnumerable<byte>> GetChunkAsync(Guid mediaId, int chunkIndex, int chunkBitrateIndex);
    public Task<Guid> InsertMediaAsync(string name, IEnumerable<byte> thumbnail, IEnumerable<byte> media);
    public Task RemoveMediaAsync(Guid mediaId);
}