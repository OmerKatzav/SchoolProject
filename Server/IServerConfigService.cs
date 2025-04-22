namespace Server
{
    internal interface IServerConfigService
    {
        public CaptchaConfig CaptchaConfig { get; set; }
        public LoginConfig LoginConfig { get; set; }
        public DbConfig DbConfig { get; set; }
        public QuicConfig QuicConfig { get; set; }
        public CryptoConfig CryptoConfig { get; set; }
    }
}
