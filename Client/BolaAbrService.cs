using Shared;
using System.Collections.Concurrent;

namespace Client;

internal class BolaAbrService(IClientConfigService configService) : IAbrService
{
    private readonly ConcurrentDictionary<Guid, int[]> _mStar = [];
    private readonly ConcurrentDictionary<Guid, BolaParams?[]> _bolaParams = [];
    private BolaConfig BolaConfig => configService.BolaConfig ?? throw new ArgumentNullException(nameof(configService.BolaConfig));

    public async Task<int> GetNewChunkIdAsync(Guid mediaId, ChunkMetadata chunkMetadata, int chunkIndex, double currentTime, double bufferSize, double lastBandwidth)
    {
        var parameters = GetParams(mediaId, chunkMetadata, chunkIndex);
        var t = Math.Min(currentTime, parameters.Length);
        var tTag = Math.Max(t / 2, 3 * parameters.P);
        var qDMax = Math.Min(parameters.QMax, tTag / parameters.P);
        var vD = (qDMax - 1) / (parameters.Utilities[^1] + parameters.Gamma * parameters.P);
        if (!_mStar.TryGetValue(mediaId, out var mStarValue))
        {
            mStarValue = new int[(int)Math.Ceiling(parameters.Length / parameters.P)];
            _mStar.TryAdd(mediaId, mStarValue);
        }
        var valueArr = Enumerable.Range(0, parameters.S.Length - 1).Select(i => (vD * parameters.Utilities[i] + vD * parameters.Gamma * parameters.P - bufferSize) / parameters.S[i]).ToArray();
        for (var i = 0; i < valueArr.Length; i++)
        {
            if (valueArr[i] > valueArr[mStarValue[chunkIndex]])
            {
                mStarValue[chunkIndex] = i;
            }
        }
        if (chunkIndex == 0 || mStarValue[chunkIndex] >= mStarValue[chunkIndex - 1])
        {
            var mTag = parameters.S.Where(sM => sM / parameters.P <= Math.Max(lastBandwidth, parameters.S[0] / parameters.P)).Select((_, index) => index).Max();
            if (mTag >= mStarValue[chunkIndex]) mTag = mStarValue[chunkIndex];
            else if (mTag < mStarValue[chunkIndex - 1]) mTag = mStarValue[chunkIndex - 1];
            mStarValue[chunkIndex] = mTag;
        }
        await Task.Delay((int)Math.Max(parameters.P * (bufferSize - qDMax + 1) * 1000, 0));
        return Math.Clamp(mStarValue[chunkIndex], 0, parameters.S.Length - 1);
    }

    public bool ShallAbandon(Guid mediaId, ChunkMetadata chunkMetadata, int chunkIndex, double currentTime, double bufferSize, int bitrateIndex, int bytesLeft)
    {
        if (bitrateIndex == 0) return false;
        var parameters = GetParams(mediaId, chunkMetadata, chunkIndex);
        var t = Math.Min(currentTime, parameters.Length);
        var tTag = Math.Max(t / 2, 3 * parameters.P);
        var qDMax = Math.Min(parameters.QMax, tTag / parameters.P);
        var vD = (qDMax - 1) / (parameters.Utilities[^1] + parameters.Gamma * parameters.P);
        var rm = (vD * parameters.Utilities[bitrateIndex] + vD * parameters.Gamma * parameters.P - bufferSize) / bytesLeft;
        //return Enumerable.Range(0, bitrateIndex - 1).Select(i => (vD * parameters.Utilities[i] + vD * parameters.Gamma * parameters.P - bufferSize) / parameters.S[i]).Any(rmTag => rmTag > rm);
        return false;
    }

    public void StopMedia(Guid mediaId)
    {
        _mStar.TryRemove(mediaId, out _);
        _bolaParams.TryRemove(mediaId, out _);
    }

    private BolaParams GetParams(Guid mediaId, ChunkMetadata metadata, int chunkIndex)
    {
        if (_bolaParams.TryGetValue(mediaId, out var bolaParams) && chunkIndex < bolaParams.Length && bolaParams[chunkIndex] is not null)
        {
            return bolaParams[chunkIndex]!;
        }
        var p = metadata.SecondsPerChunk;
        var chunkSizes = metadata.ChunkSizes ?? throw new ArgumentNullException(nameof(metadata));
        var s = Enumerable.Range(0, chunkSizes.GetLength(1)).Select(j => chunkSizes[chunkIndex, j]).ToArray();
        var qMax = BolaConfig.MaxBufferSize / p;
        var qMin = BolaConfig.MinBufferSize / p;
        var utilities = s.Select(size => Math.Log((double)size / s[^1])).ToArray();
        var alpha = (s[0] * utilities[1] - s[1] * utilities[0]) / (s[1] - s[0]);
        var gamma = (utilities[^1] * qMin - alpha * qMax) / (qMax - qMin) / p;
        var length = metadata.Length;
        bolaParams = new BolaParams[(int)Math.Ceiling(length / p)];
        bolaParams[chunkIndex] = new BolaParams(p, s, qMax, utilities, gamma, length);
        _bolaParams.TryAdd(mediaId, bolaParams);
        return bolaParams[chunkIndex]!;
    }
}