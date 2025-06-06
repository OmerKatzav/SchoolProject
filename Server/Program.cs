using DI;
using dotenv.net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Minio;
using RPC;
using Server;
using Shared;
using SixLaborsCaptcha.Core;
using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Formats.Png;

if (!QuicConnection.IsSupported || !QuicListener.IsSupported) throw new NotSupportedException("QUIC is not supported on this platform");

var serviceProvider = new ServiceProvider();
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var ecdsa = ECDsa.Create();
ecdsa.ImportFromPem(File.ReadAllText("signing_key.pem"));
var quicConnectionOptions = new QuicServerConnectionOptions
{
    ServerAuthenticationOptions = new SslServerAuthenticationOptions
    {
        ApplicationProtocols = [new SslApplicationProtocol("project/1.0")],
        ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile("server-cert.pfx", "PfxPassword")
    },
    DefaultCloseErrorCode = 0,
    DefaultStreamErrorCode = 1
};
DotEnv.Load();
var dotenvVars = DotEnv.Read();
var minioFactory = new MinioClientFactory(client => client
    .WithEndpoint(dotenvVars["MINIO_ENDPOINT"])
    .WithCredentials(dotenvVars["MINIO_ACCESS_KEY"], dotenvVars["MINIO_SECRET_KEY"])
    .WithSSL());

serviceProvider.RegisterSingleton<IServerConfigService>(() =>
{
    if (!QuicConnection.IsSupported || !QuicListener.IsSupported) throw new NotSupportedException("QUIC is not supported on this platform");
    return new ServerConfigService
    {
        CaptchaConfig = new CaptchaConfig(new SixLaborsCaptchaOptions(), 6, TimeSpan.FromMinutes(2)),
        CryptoConfig = new Server.CryptoConfig(ecdsa, HashAlgorithmName.SHA256, 32, 16),
        DbConfig = new DbConfig(new DbContextOptionsBuilder().UseNpgsql(dotenvVars["NPGSQL_CONNECTION_STRING"]).Options),
        LoginConfig = new LoginConfig(
            TimeSpan.FromDays(7),
            TimeSpan.FromMinutes(10),
            new Regex("^[A-Za-z0-9_-]{3,16}$"),
            new Regex(@"^(?=.{8,128}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()])[A-Za-z\d!@#$%^&*()]+$"),
            "Username must be 3-16 characters and contain only alphanumeric characters and -,_",
            "Password must be 8-128 characters and contain at least one lowercase character, uppercase character and special character",
            "[Project] Your Login Verification Code",
            "[Project] Confirm Your Registration",
            "[Project] Password Reset Request",
            "[Project] Confirm Your New Email Address",
            "[Project] Confirm Your New Password Request",
            (username, code) =>
                $"""
                 Hi {username},

                 We received a request to log in to your account on Project.

                 To complete the login, please copy the code below and paste it into the app:

                 {code}

                 This code will expire in 10 minutes.

                 If you did not attempt to log in, please reset your password now.

                 Thanks,  
                 The Project Team
                 """,
            (username, code) =>
                $"""
                 Hi {username},

                 Thank you for signing up for Project!

                 To complete your registration and activate your account, please copy the code below and paste it into the app:

                 {code}

                 This code will expire in 10 minutes.

                 If you did not sign up for Project, you can safely ignore this email.

                 Thanks,
                 The Project Team
                 """,
            (username, code) =>
                $"""
                 Hi {username},
                 
                 We received a request to reset the password for your Project account.
                 
                 To reset your password, please copy the code below and paste it into the app:
                 
                 {code}
                 
                 This code will expire in 10 minutes.
                 
                 If you did not request a password reset, you can safely ignore this email.
                 
                 Thanks,  
                 The Project Team
                 """,
            (username, code) =>
                $"""
                 Hi {username},
                 
                 We received a request to change the email address associated with your {username} account to this one.
                 
                 To confirm this change, please copy the code below and paste it into the app:
                 
                 {code}
                 
                 This code will expire in 10 minutes.
                 
                 If you did not request this email change, you can safely ignore this email.
                 
                 Thanks,  
                 The Project Team
                 """,
            (username, code) =>
                $"""
                 Hi {username},
                 
                 We received a request to change the password for your Project account.
                 
                 To confirm this change, please copy the code below and paste it into the app:
                 
                 {code}
                 
                 This code will expire in 10 minutes.
                 
                 If you did not request this password change, you can safely ignore this email.
                 
                 Thanks,  
                 The Project Team
                 """,
            (username, code) =>
                $"""
                 <p>Hi {username},</p>

                 <p>We received a request to log in to your account on <strong>Project</strong>.</p>

                 <p>To complete the login, please copy the code below and paste it into the app:</p>

                 <pre style="background-color:#f3f3f3; padding: 10px; border: 1px solid #ccc;">
                 <code style="word-break: break-all;">{code}</code>
                 </pre>

                 <p><strong>This code will expire in 10 minutes.</strong></p>

                 <p>If you did not attempt to log in, please reset your password now.</p>
                 
                 <p>Thanks,<br>The Project Team</p>
                 """,
            (username, code) =>
                $"""
                 <p>Hi <strong>{username}</strong>,</p>

                 <p>Thank you for signing up for <strong>Project</strong>!</p>

                 <p>To complete your registration and activate your account, please copy the code below and paste it into the app:</p>

                 <pre style="background-color:#f3f3f3; padding: 10px; border: 1px solid #ccc;">
                 <code style="word-break: break-all;">{code}</code>
                 </pre>

                 <p><strong>This code will expire in 10 minutes.</strong></p>

                 <p>If you did not sign up for Project, you can safely ignore this email.</p>

                 <p>Thanks,<br>
                 The Project Team</p>
                 """,
            (username, code) =>
                $"""
                 <p>Hi <strong>{username}</strong>,</p>
                 
                 <p>We received a request to reset the password for your <strong>Project</strong> account.</p>
                 
                 <p>To reset your password, please copy the code below and paste it into the app:</p>
                 
                 <pre style="background-color:#f3f3f3; padding: 10px; border: 1px solid #ccc; white-space: pre-wrap;">
                 <code style="word-break: break-all;">{code}</code>
                 </pre>
                 
                 <p><strong>This code will expire in 10 minutes.</strong></p>
                 
                 <p>If you did not request a password reset, you can safely ignore this email.</p>
                 
                 <p>Thanks,<br>
                 The Project Team</p>
                 """,
            (username, code) =>
                $"""
                 <p>Hi <strong>{username}</strong>,</p>
                 
                 <p>We received a request to change the email address associated with your <strong>Project</strong> account to this one:</p>
                 
                 <p>To confirm this change, please copy the code below and paste it into the app:</p>
                 
                 <pre style="background-color:#f3f3f3; padding: 10px; border: 1px solid #ccc; white-space: pre-wrap;">
                 <code style="word-break: break-all;">{code}</code>
                 </pre>
                 
                 <p><strong>This code will expire in 10 minutes.</strong></p>
                 
                 <p>If you did not request this change, you can safely ignore this email.</p>
                 
                 <p>Thanks,<br>
                 The YourApp Team</p>
                 """,
            (username, code) =>
                $"""
                 <p>Hi <strong>{username}</strong>,</p>
                 
                 <p>We received a request to change the password for your <strong>Project</strong> account.</p>
                 
                 <p>To confirm this change, please copy the code below and paste it into the app:</p>
                 
                 <pre style="background-color:#f3f3f3; padding: 10px; border: 1px solid #ccc; white-space: pre-wrap;">
                 <code style="word-break: break-all;">{code}</code>
                 </pre>
                 
                 <p><strong>This code will expire in 10 minutes.</strong></p>
                 
                 <p>If you did not request this change, you can safely ignore this email.</p>
                 
                 <p>Thanks,<br>
                 The Project Team</p>
                 """
        ),
        QuicConfig = new QuicConfig(new QuicListenerOptions
        {
            ListenEndPoint = new IPEndPoint(IPAddress.Any, 1234),
            ApplicationProtocols = [new SslApplicationProtocol("project/1.0")],
            ConnectionOptionsCallback = (_, _, _) =>
            {
                if (!QuicConnection.IsSupported || !QuicListener.IsSupported) throw new NotSupportedException("QUIC is not supported on this platform");
                return ValueTask.FromResult(quicConnectionOptions);
            }
        }),
        EmailConfig = new EmailConfig("Project Team", dotenvVars["EMAIL_ADDRESS"], "smtp.google.com", 587, SecureSocketOptions.StartTls,
            async () =>
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = dotenvVars["EMAIL_CLIENT_ID"],
                    ClientSecret = dotenvVars["EMAIL_CLIENT_SECRET"]
                };

                var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    DataStore = new FileDataStore("CredentialCacheFolder"),
                    Scopes = ["https://mail.google.com/"],
                    ClientSecrets = clientSecrets,
                    LoginHint = dotenvVars["GMAIL_ACCOUNT"]
                });

                var codeReceiver = new LocalServerCodeReceiver();
                var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

                var credential = await authCode.AuthorizeAsync(dotenvVars["GMAIL_ACCOUNT"], CancellationToken.None);

                if (credential.Token.IsStale)
                    await credential.RefreshTokenAsync(CancellationToken.None);

                return new SaslMechanismOAuthBearer(credential.UserId, credential.Token.AccessToken);
            }),
        MediaConfig = new MediaConfig("thumbnails", "chunks", [24000, 96000, 16000, 320000], 48000, 2, "libopus", "opus", "flac", "flac", "Transcodes", -14, -1.5, 11, (512, 512), new PngEncoder())
    };
});
serviceProvider.RegisterSingleton(() => loggerFactory.CreateLogger(""));
serviceProvider.RegisterSingleton(() => minioFactory.CreateClient());
serviceProvider.RegisterSingleton<IRequestHandler, RpcRequestHandler>();
serviceProvider.RegisterSingleton<DbContext, AppDbContext>();
serviceProvider.RegisterSingleton<ICryptoService, CryptoService>();
serviceProvider.RegisterSingleton<ICaptchaService, CaptchaService>();
serviceProvider.RegisterSingleton<IUserService, UserService>();
serviceProvider.RegisterSingleton<IServer, QuicServer>();
serviceProvider.RegisterSingleton<ILoginService, LoginService>();
serviceProvider.RegisterSingleton<IMediaInternalService, MediaInternalService>();
serviceProvider.RegisterSingleton<IMediaService, MediaService>();

await serviceProvider.GetService<DbContext>().Database.MigrateAsync();
await serviceProvider.GetService<IServer>().StartAsync();
