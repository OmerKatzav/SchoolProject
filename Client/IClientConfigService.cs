namespace Client
{
    internal interface IClientConfigService
    {
        public QuicConfig QuicConfig { get; set; }
        public BolaConfig BolaConfig { get; set; }
    }
}
