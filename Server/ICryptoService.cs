namespace Server;

internal interface ICryptoService
{
    public IEnumerable<byte> SignBytes(IEnumerable<byte> bytes);
    public bool VerifySignature(IEnumerable<byte> bytes, IEnumerable<byte> signature);
    public IEnumerable<byte> HashBytes(IEnumerable<byte> bytes, IEnumerable<byte> salt, IEnumerable<byte> associatedData);
    public IEnumerable<byte> GenerateSalt();
}