using SixLabors.ImageSharp.Formats;

namespace Server;

internal class MediaConfig(string thumbnailBucketName, string chunkBucketName, int[] bitrates, int sampleRate, double secondsPerChunk, string audioCodec, string audioCodecContainer, string losslessAudioCodec, string losslessAudioContainer, string tempPath, double targetLufs, double targetTp, double targetLra, (int, int) thumbnailResolution, IImageEncoder thumbnailEncoder)
{
    public string ThumbnailBucketName { get; } = thumbnailBucketName;
    public string ChunkBucketName { get; } = chunkBucketName;
    public int[] Bitrates { get; } = bitrates;
    public int SampleRate { get; } = sampleRate;
    public double SecondsPerChunk { get; } = secondsPerChunk;
    public string AudioCodec { get; } = audioCodec;
    public string AudioCodecContainer { get; } = audioCodecContainer;
    public string LosslessAudioCodec { get; } = losslessAudioCodec;
    public string LosslessAudioContainer { get; } = losslessAudioContainer;
    public string TempPath { get; } = tempPath;
    public double TargetLufs { get; } = targetLufs;
    public double TargetTp { get; } = targetTp;
    public double TargetLra { get; } = targetLra;
    public (int, int) ThumbnailResolution { get; } = thumbnailResolution;
    public IImageEncoder ThumbnailEncoder { get; } = thumbnailEncoder;
}