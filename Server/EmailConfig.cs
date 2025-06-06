using MailKit.Security;

namespace Server;

internal class EmailConfig(string fromName, string fromAddress, string smtpServer, int smtpPort, SecureSocketOptions socketOptions, Func<Task<SaslMechanism>> getSaslAsync)
{
    public string FromName { get; } = fromName;
    public string FromAddress { get; } = fromAddress;
    public string SmtpServer { get; } = smtpServer;
    public int SmtpPort { get; } = smtpPort;
    public SecureSocketOptions SocketOptions { get; } = socketOptions;
    public Func<Task<SaslMechanism>> GetSaslAsync { get; } = getSaslAsync;
}