using System.Security.Cryptography.X509Certificates;

namespace Services;
public interface IVaultClient
{
    Task<string> GetSecretAsync(string secretName);
    Task<X509Certificate2> GetCertificateAsync(string certificateName);
}