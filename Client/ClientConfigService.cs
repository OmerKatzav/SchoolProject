namespace Client;

internal class ClientConfigService : IClientConfigService
{
    public QuicConfig? QuicConfig { get; set; }
    public BolaConfig? BolaConfig { get; set; }
    public TokenStorageConfig? TokenStorageConfig { get; set; }
}