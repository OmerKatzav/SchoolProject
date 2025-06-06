namespace Server;

internal interface IEmailService
{
    public bool IsValidEmail(string email);

    public Task SendEmailAsync(string email, string subject, string body, string? htmlBody);
}