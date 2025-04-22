namespace Server
{
    internal class ServerConfigService(CaptchaConfig captchaConfig, CryptoConfig cryptoConfig, DbConfig dbConfig, LoginConfig loginConfig, QuicConfig quicConfig) : IServerConfigService
    {
        public CaptchaConfig CaptchaConfig { get; set; } = captchaConfig;
        public CryptoConfig CryptoConfig { get; set; } = cryptoConfig;
        public DbConfig DbConfig { get; set; } = dbConfig;
        public LoginConfig LoginConfig { get; set; } = loginConfig;
        public QuicConfig QuicConfig { get; set; } = quicConfig;
    }
}
