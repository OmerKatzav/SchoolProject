using DI;
using Microsoft.Extensions.Logging;
using RPC;
using Shared;
using System.Net;
using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Castle.DynamicProxy;
using FFMpegCore;

namespace Client;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        if (!QuicConnection.IsSupported) throw new NotSupportedException("QUIC is not supported on this platform");

        var serverCert = X509CertificateLoader.LoadCertificateFromFile("server_cert.pem");
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        GlobalFFOptions.Configure(options => options.BinaryFolder = "./ffmpeg_bin");

        var serviceProvider = new ServiceProvider();
        serviceProvider.RegisterSingleton<IClientConfigService>(() =>
        {
            if (!QuicConnection.IsSupported) throw new NotSupportedException("QUIC is not supported on this platform");
            return new ClientConfigService
            {
                BolaConfig = new BolaConfig(15, 3),
                QuicConfig = new QuicConfig(
                    new QuicClientConnectionOptions
                    {
                        RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 1234),
                        DefaultCloseErrorCode = 0,
                        DefaultStreamErrorCode = 1,
                        IdleTimeout = TimeSpan.FromSeconds(30),
                        KeepAliveInterval = TimeSpan.FromSeconds(10),
                        ClientAuthenticationOptions = new SslClientAuthenticationOptions
                        {
                            ApplicationProtocols = [new SslApplicationProtocol("project/1.0")],
                            RemoteCertificateValidationCallback = (_, certificate, _, _) => certificate is not null &&
                                certificate.GetCertHash().SequenceEqual(serverCert.GetCertHash())
                        }
                    }, 
                    4096),
                TokenStorageConfig = new TokenStorageConfig(new FileStream("token_store.bin", FileMode.OpenOrCreate))
            };
        });
        serviceProvider.RegisterSingleton(() => loggerFactory.CreateLogger(""));
        serviceProvider.RegisterSingleton<IProxyGenerator, ProxyGenerator>();
        serviceProvider.RegisterSingleton<IProxyBuilder, DefaultProxyBuilder>();
        serviceProvider.RegisterSingleton<INetworkService, QuicNetworkService>();
        serviceProvider.RegisterSingleton<IAbrService, BolaAbrService>();
        serviceProvider.RegisterSingleton<ITokenStorageService, TokenStorageService>();
        serviceProvider.RegisterRpcSingleton<ICaptchaService>();
        serviceProvider.RegisterRpcSingleton<ILoginService>();
        serviceProvider.RegisterRpcSingleton<IMediaService>();

        var token = serviceProvider.GetService<ITokenStorageService>().AuthToken;

        ApplicationConfiguration.Initialize();
        if (token is null || !serviceProvider.GetService<ILoginService>().ValidateTokenAsync(token).Result)
        {
            var loginForm = new LoginForm(serviceProvider);
            Application.Run(loginForm);
            if (loginForm.DialogResult != DialogResult.OK)
            {
                MessageBox.Show(@"Login failed or cancelled. Please try again.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        Application.Run(new MainForm(serviceProvider));
    }
}