namespace Microsoft.AspNetCore.Hosting;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static class WebHostBuilderExtensions
{
    //
    // Summary:
    //     Configures Kestrel options but does not register an IServer.
    //
    // Parameters:
    //   builder:
    //     The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
    //
    // Returns:
    //     The Microsoft.AspNetCore.Hosting.IWebHostBuilder.
    public static IWebHostBuilder ConfigureHttps(this IWebHostBuilder builder) => builder.ConfigureKestrel(
        kestrelOptions =>
        {
            var certFile = "tls.crt";
            var keyFile = "tls.key";
            kestrelOptions.ConfigureHttpsDefaults(listenOptions =>
            {
                using var privateKey = RSA.Create();
                privateKey.ImportRSAPrivateKey(PemBytes(keyFile), out var bytesRead);
                var certificate = new X509Certificate2(PemBytes(certFile));
                listenOptions.ServerCertificate = new X509Certificate2(
                    certificate.CopyWithPrivateKey(privateKey).Export(X509ContentType.Pfx));
            });
            kestrelOptions.Limits.MaxRequestBodySize = Int32.MaxValue;
        });

    private static Byte[] PemBytes(String fileName) => Convert.FromBase64String(
        File.ReadAllLines(fileName)
            .Where(l => !l.Contains('-')).Where(l => !l.Contains(' '))
            .Aggregate("", (current, next) => current + next)
    );
}