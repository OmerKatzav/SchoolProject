using Shared;

namespace Client
{
    internal class BolaAbrService(IClientConfigService configService, IMediaService mediaService) : IAbrService
    {
        public Task<byte[]> GetNewChunkAsync(Guid mediaId, ChunkMetadata metadata, int chunkIndex,
            double currentTime, AuthToken authToken)
        {
            var p = metadata.SecondsPerChunk;
            var qMax = configService.BolaConfig.MaxBufferSize;
            var qMin = configService.BolaConfig.MinBufferSize;
            var utilities = metadata.ChunkBitrates.Select(bitrate => Math.Log(bitrate)).ToArray();
            var v = p * (qMax - qMin) / (utilities[-1] - utilities[0]);
            var gamma = p * qMin / v - utilities[0];
            var vMin = configService.BolaConfig.VMinCoef * v;
            var vMax = configService.BolaConfig.VMaxCoef * v;
            var vDt = vMin + (vMax - vMin) * (currentTime / metadata.Length);
            var qI = utilities.Select(ui => vDt * (ui + gamma) / p);
            var bitrateIndex = qI.Where(qi => qi < currentTime).OrderDescending().Select((_, index) => index)
                .FirstOrDefault();
            return mediaService.GetChunk(authToken, mediaId, chunkIndex, bitrateIndex);
        }
    }
}
