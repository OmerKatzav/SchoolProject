namespace Server;

internal interface IStoreService
{
    public Task StoreAsync(string bucket, string key, Stream stream);

    public Task StoreAsync(string bucket, string key, string file);

    public Task DeleteAsync(string bucket, string key);

    public Task<Stream> RetrieveAsync(string bucket, string key);
}