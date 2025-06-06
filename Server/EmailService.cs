using MailKit.Net.Smtp;
using MimeKit;

namespace Server;

internal class EmailService(IServerConfigService configService) : IEmailService
{
    private EmailConfig EmailConfig => configService.EmailConfig ?? throw new ArgumentNullException(nameof(configService.EmailConfig));

    public async Task SendEmailAsync(string email, string subject, string body, string? htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(EmailConfig.FromName, EmailConfig.FromAddress));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = subject;
        var bodyBuilder = new BodyBuilder
        {
            TextBody = body,
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(EmailConfig.SmtpServer, EmailConfig.SmtpPort, EmailConfig.SocketOptions);
        await client.AuthenticateAsync(await EmailConfig.GetSaslAsync());
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public bool IsValidEmail(string email)
    {
        return MailboxAddress.TryParse(email, out _) && email.Length <= 254;
    }
}