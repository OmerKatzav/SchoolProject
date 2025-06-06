using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.EntityFrameworkCore;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text;
using System.Text.Json;

namespace Server;

internal class MediaInternalService(IStoreService storeService, AppDbContext dbContext, IServerConfigService configService) : IMediaInternalService
{
    private MediaConfig MediaConfig => configService.MediaConfig ?? throw new ArgumentNullException(nameof(configService.MediaConfig));

    public IEnumerable<Guid> GetMediaIds()
    {
        return dbContext.Medias.Select(m => m.Id);
    }

    public IEnumerable<Guid> SearchMedia(string searchTerm)
    {
        return dbContext.Medias
            .Select(m => new
            {
                Media = m,
                Rank = m.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", searchTerm))
            })
            .Where(x => x.Rank > 0)
            .OrderByDescending(x => x.Rank)
            .Select(x => x.Media.Id);
    }

    public async Task<string> GetMediaNameAsync(Guid mediaId)
    {
        var media = await dbContext.Medias.FindAsync(mediaId) ?? throw new ArgumentException("Media not found");
        return media.Name;
    }

    public async Task<IEnumerable<byte>> GetMediaThumbnailAsync(Guid mediaId)
    {
        var thumbnailStream = await storeService.RetrieveAsync(MediaConfig.ThumbnailBucketName, mediaId.ToString());
        var buffer = new byte[thumbnailStream.Length];
        await thumbnailStream.ReadExactlyAsync(buffer, 0, (int)thumbnailStream.Length);
        return buffer;
    }

    public async Task<ChunkMetadata> GetChunkMetadataAsync(Guid mediaId)
    {
        var media = await dbContext.Medias.FindAsync(mediaId) ?? throw new ArgumentException("Media not found");
        return new ChunkMetadata
        {
            ChunkSizes = media.ChunkSizes,
            SecondsPerChunk = media.SecondsPerChunk,
            Length = media.Length,
        };
    }

    public async Task<IEnumerable<byte>> GetChunkAsync(Guid mediaId, int chunkIndex, int chunkBitrateIndex)
    {
        var chunkStream = await storeService.RetrieveAsync(MediaConfig.ChunkBucketName, $"{mediaId}/{chunkIndex}/{chunkBitrateIndex}");
        var buffer = new byte[chunkStream.Length];
        await chunkStream.ReadExactlyAsync(buffer, 0, (int)chunkStream.Length);
        return buffer;
    }

    public async Task<Guid> InsertMediaAsync(string name, IEnumerable<byte> thumbnail, IEnumerable<byte> media)
    {
        var encodedThumbnailStream = new MemoryStream();
        using (var image = await Image.LoadAsync(new MemoryStream([.. thumbnail])))
        {
            image.Mutate(x => x.Resize(MediaConfig.ThumbnailResolution.Item1, MediaConfig.ThumbnailResolution.Item2));
            await image.SaveAsync(encodedThumbnailStream, MediaConfig.ThumbnailEncoder);
        }
        encodedThumbnailStream.Position = 0;

        var mediaStream = new MemoryStream([.. media], false);

        var analysis = await FFProbe.AnalyseAsync(mediaStream, new FFOptions());
        if (analysis.AudioStreams.Count == 0) throw new ArgumentException("No audio stream found in the media file.");
        mediaStream.Position = 0;

        var sb = new StringBuilder();
        var isInJson = false;
        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(mediaStream))
            .OutputToPipe(new StreamPipeSink(Stream.Null), o => o
                .WithCustomArgument($"-af loudnorm=I={MediaConfig.TargetLufs}:TP={MediaConfig.TargetTp}:LRA={MediaConfig.TargetLra}:print_format=json")
                .ForceFormat("null"))
            .WithLogLevel(FFMpegLogLevel.Info)
            .NotifyOnError(s =>
            {
                if (isInJson) sb.AppendLine(s);
                if (s.Contains("Parsed_loudnorm")) isInJson = true;
                if (s.Contains('}')) isInJson = false;
            })
            .ProcessAsynchronously();
        var loudnormStats = JsonSerializer.Deserialize<LoudnormStats>(sb.ToString()) ?? throw new InvalidOperationException("Failed to parse FFMpeg loudnorm json");
        var normString = $"-af loudnorm=I={MediaConfig.TargetLufs}:TP={MediaConfig.TargetTp}:LRA={MediaConfig.TargetLra}:measured_I={loudnormStats.InputI}:measured_TP={loudnormStats.InputTp}:measured_LRA={loudnormStats.InputLra}:measured_thresh={loudnormStats.InputThresh}:offset={loudnormStats.TargetOffset}:linear=true";
        mediaStream.Position = 0;

        var chunkSizes = new int[(int)Math.Ceiling(analysis.Duration.TotalSeconds / MediaConfig.SecondsPerChunk), MediaConfig.Bitrates.Length + 1];
        var mediaId = Guid.NewGuid();
        var mediaPath = Path.Combine(MediaConfig.TempPath, mediaId.ToString());

        Directory.Delete(mediaPath, true);

        var bestAudioIndex = 0;
        var stereoStreamIndices = analysis.AudioStreams.Select((_, i) => i).Where(i => analysis.AudioStreams[i].Channels == 2).ToArray();
        if (stereoStreamIndices.Length == 0) stereoStreamIndices = [.. analysis.AudioStreams.Select((_, i) => i)];
        foreach (var audioIndex in stereoStreamIndices)
            if (analysis.AudioStreams[audioIndex].BitRate > analysis.AudioStreams[bestAudioIndex].BitRate) bestAudioIndex = audioIndex;

        var bitrates = MediaConfig.Bitrates.Where(bitrate => bitrate < analysis.AudioStreams[bestAudioIndex].BitRate).ToArray();

        var jobs = new List<Task>();
        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(mediaStream),
                o => o.SelectStream(bestAudioIndex, channel: Channel.Audio))
            .MultiOutput(o =>
            {
                var newO = o.OutputToFile(Path.Combine(mediaPath, $"%d_{MediaConfig.Bitrates.Length}"), true, a => a
                    .WithAudioCodec(MediaConfig.LosslessAudioCodec)
                    .WithCustomArgument("-f segment")
                    .WithCustomArgument($"-segment time {MediaConfig.SecondsPerChunk}")
                    .WithCustomArgument("-reset_timestamps 1")
                    .WithCustomArgument(normString)
                    .ForceFormat(MediaConfig.LosslessAudioContainer));
                for (var bitrateIndex = 0; bitrateIndex < bitrates.Length; bitrateIndex++)
                {
                    var bitrate = bitrates[bitrateIndex];
                    newO.OutputToFile(Path.Combine(mediaPath, $"%d_{bitrateIndex}"), true, a => a
                        .WithAudioCodec(MediaConfig.AudioCodec)
                        .WithAudioBitrate(bitrate)
                        .WithAudioSamplingRate(MediaConfig.SampleRate)
                        .WithCustomArgument("-f segment")
                        .WithCustomArgument($"-segment time {MediaConfig.SecondsPerChunk}")
                        .WithCustomArgument("-reset_timestamps 1")
                        .WithCustomArgument(normString)
                        .ForceFormat(MediaConfig.AudioCodecContainer));
                }
            })
            .ProcessAsynchronously();

        foreach (var file in Directory.GetFiles(mediaPath))
        {
            var id = Path.GetFileName(file).Split('_');
            var chunkIndex = id[0];
            var chunkBitrateIndex = id[1];
            chunkSizes[int.Parse(chunkIndex), int.Parse(chunkBitrateIndex)] = (int)new FileInfo(file).Length;
            jobs.Add(storeService.StoreAsync(MediaConfig.ChunkBucketName, $"{mediaId}/{chunkIndex}/{chunkBitrateIndex}", file));
        }

        await dbContext.Medias.AddAsync(new Media
        {
            Id = mediaId,
            Name = name,
            Length = analysis.Duration.TotalSeconds,
            SecondsPerChunk = MediaConfig.SecondsPerChunk,
            ChunkSizes = chunkSizes
        });

        jobs.Add(storeService.StoreAsync(MediaConfig.ThumbnailBucketName, mediaId.ToString(), encodedThumbnailStream));
        jobs.Add(dbContext.SaveChangesAsync());

        await Task.WhenAll(jobs);

        Directory.Delete(mediaPath, true);

        return mediaId;
    }

    public async Task RemoveMediaAsync(Guid mediaId)
    {
        var jobs = new List<Task>();
        var media = await dbContext.Medias.FindAsync(mediaId) ?? throw new ArgumentException("Media not found");

        for (var chunkIndex = 0; chunkIndex < media.ChunkSizes.GetLength(0); chunkIndex++)
            for (var bitrateIndex = 0; bitrateIndex < media.ChunkSizes.GetLength(1); bitrateIndex++)
                jobs.Add(storeService.DeleteAsync(MediaConfig.ChunkBucketName, $"{mediaId}/{chunkIndex}/{bitrateIndex}"));

        dbContext.Medias.Remove(media);
        jobs.Add(dbContext.SaveChangesAsync());
        jobs.Add(storeService.DeleteAsync(MediaConfig.ThumbnailBucketName, mediaId.ToString()));
        await Task.WhenAll(jobs);
    }
}