using Minio;
using Minio.DataModel.Args;

namespace Server;

internal class MinioStoreService(IMinioClient minioClient) : IStoreService
{
    public async Task StoreAsync(string bucket, string key, Stream stream)
    {
        await minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
        );
    }

    public async Task StoreAsync(string bucket, string key, string file)
    {
        await minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
            .WithFileName(file)
        );
    }

    public async Task DeleteAsync(string bucket, string key)
    {
        await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
        );
    }

    public async Task<Stream> RetrieveAsync(string bucket, string key)
    {
        var returnStream = Stream.Null;
        await minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
            .WithCallbackStream(stream => returnStream = stream)
        );
        return returnStream;
    }
}
