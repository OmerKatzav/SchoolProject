using RPC;
using Shared;

namespace Server;

internal class MediaService(IMediaInternalService mediaInternalService, ILoginService loginService) : IMediaService
{
    public async Task<Guid[]> GetMediaIdsAsync(AuthToken authToken)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return [.. mediaInternalService.GetMediaIds()];
    }

    public async Task<Guid[]> SearchMediasAsync(AuthToken authToken, string searchTerm)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return [.. mediaInternalService.SearchMedia(searchTerm)];
    }

    public async Task<string> GetMediaNameAsync(AuthToken authToken, Guid mediaId)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return await mediaInternalService.GetMediaNameAsync(mediaId);
    }

    public async Task<byte[]> GetMediaThumbnailAsync(AuthToken authToken, Guid mediaId)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return [.. (await mediaInternalService.GetMediaThumbnailAsync(mediaId))];
    }

    public async Task<ChunkMetadata> GetChunkMetadataAsync(AuthToken authToken, Guid mediaId)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return await mediaInternalService.GetChunkMetadataAsync(mediaId);
    }

    public async Task<byte[]> GetChunkAsync(AuthToken authToken, Guid mediaId, int chunkIndex,
        int chunkBitrateIndex, Action<CallMetadata>? downloadCallback = null, CancellationToken ctx = default)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess)
            throw new ArgumentException("Invalid auth token.");
        return [.. (await mediaInternalService.GetChunkAsync(mediaId, chunkIndex, chunkBitrateIndex))];
    }

    public async Task<Guid> InsertMediaAsync(AuthToken authToken, string name, byte[] thumbnail, byte[] media)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess || !await loginService.IsAdminAsync(authToken)) throw new ArgumentException("Invalid auth token.");
        return await mediaInternalService.InsertMediaAsync(name, thumbnail, media);
    }

    public async Task RemoveMediaAsync(AuthToken authToken, Guid mediaId)
    {
        if (!await loginService.ValidateTokenAsync(authToken) || authToken.Purpose != AuthTokenPurpose.FullAccess || !await loginService.IsAdminAsync(authToken)) throw new ArgumentException("Invalid auth token.");
        await mediaInternalService.RemoveMediaAsync(mediaId);
    }
}