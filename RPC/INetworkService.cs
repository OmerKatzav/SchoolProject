namespace RPC;

public interface INetworkService
{
    public Task StartAsync();
    public IEnumerable<byte> Request(IEnumerable<byte> data, Action<CallMetadata>? callback = null, CancellationToken ctx = default);
    public Task<IEnumerable<byte>> RequestAsync(IEnumerable<byte> data, Action<CallMetadata>? callback = null, CancellationToken ctx = default);
}