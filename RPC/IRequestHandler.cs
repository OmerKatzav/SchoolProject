namespace RPC
{
    public interface IRequestHandler
    {
        public IEnumerable<byte> HandleRequest(IEnumerable<byte> request);
    }
}
