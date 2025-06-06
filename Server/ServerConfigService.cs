namespace Server;

internal class ServerConfigService : IServerConfigService
{
    public CaptchaConfig? CaptchaConfig { get; set; }
    public CryptoConfig? CryptoConfig { get; set; }
    public DbConfig? DbConfig { get; set; }
    public LoginConfig? LoginConfig { get; set; }
    public QuicConfig? QuicConfig { get; set; }
    public EmailConfig? EmailConfig { get; set; }
    public MediaConfig? MediaConfig { get; set; }
}