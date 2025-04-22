namespace RPC;

public interface INetworkService
{
    public Task StartAsync();
    public IEnumerable<byte> Request(IEnumerable<byte> data);
    public Task<IEnumerable<byte>> RequestAsync(IEnumerable<byte> data);
}