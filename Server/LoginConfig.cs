using System.Text.RegularExpressions;

namespace Server;

internal class LoginConfig(TimeSpan expiration, TimeSpan temporaryExpiration, Regex usernameRegex, Regex passwordRegex, string usernameRegexError, string passwordRegexError, string emailLoginSubject, string emailRegisterSubject, string emailForgotPasswordSubject, string emailChangeEmailSubject, string emailChangePasswordSubject, Func<string, string, string> emailLoginBodyCreator, Func<string, string, string> emailRegisterBodyCreator, Func<string, string, string> emailForgotPasswordBodyCreator, Func<string, string, string> emailChangeEmailBodyCreator, Func<string, string, string> emailChangePasswordBodyCreator, Func<string, string, string>? emailLoginHtmlBodyCreator = null, Func<string, string, string>? emailRegisterHtmlBodyCreator = null, Func<string, string, string>? emailForgotPasswordHtmlBodyCreator = null, Func<string, string, string>? emailChangeEmailHtmlBodyCreator = null, Func<string, string, string>? emailChangePasswordHtmlBodyCreator = null)
{
    public TimeSpan Expiration { get; } = expiration;
    public TimeSpan TemporaryExpiration { get; } = temporaryExpiration;
    public Regex UsernameRegex { get; } = usernameRegex;
    public Regex PasswordRegex { get; } = passwordRegex;
    public string UsernameRegexError { get; } = usernameRegexError;
    public string PasswordRegexError { get; } = passwordRegexError;
    public string EmailLoginSubject { get; } = emailLoginSubject;
    public string EmailRegisterSubject { get; } = emailRegisterSubject;
    public string EmailForgotPasswordSubject { get; } = emailForgotPasswordSubject;
    public string EmailChangeEmailSubject { get; } = emailChangeEmailSubject;
    public string EmailChangePasswordSubject { get; } = emailChangePasswordSubject;
    public Func<string, string, string> EmailLoginBodyCreator { get; } = emailLoginBodyCreator;
    public Func<string, string, string> EmailRegisterBodyCreator { get; } = emailRegisterBodyCreator;
    public Func<string, string, string> EmailForgotPasswordBodyCreator { get; } = emailForgotPasswordBodyCreator;
    public Func<string, string, string> EmailChangeEmailBodyCreator { get; } = emailChangeEmailBodyCreator;
    public Func<string, string, string> EmailChangePasswordBodyCreator { get; } = emailChangePasswordBodyCreator;
    public Func<string, string, string>? EmailLoginHtmlBodyCreator { get; } = emailLoginHtmlBodyCreator;
    public Func<string, string, string>? EmailRegisterHtmlBodyCreator { get; } = emailRegisterHtmlBodyCreator;
    public Func<string, string, string>? EmailForgotPasswordHtmlBodyCreator { get; } = emailForgotPasswordHtmlBodyCreator;
    public Func<string, string, string>? EmailChangeEmailHtmlBodyCreator { get; } = emailChangeEmailHtmlBodyCreator;
    public Func<string, string, string>? EmailChangePasswordHtmlBodyCreator { get; } = emailChangePasswordHtmlBodyCreator;
}