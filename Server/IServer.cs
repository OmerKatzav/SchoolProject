namespace Server
{
    internal interface IServer
    {
        public Task StartAsync();
        public Task StopAsync();
    }
}
