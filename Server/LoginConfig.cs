namespace Server
{
    internal class LoginConfig(TimeSpan expiration)
    {
        public TimeSpan Expiration { get; } = expiration;
    }
}
