using System.Security.Cryptography.X509Certificates;
using Azure;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Services;
using VaultConnection.Options;

namespace VaultConnection;
public class VaultService : IVaultClient
{
    private readonly CertificateClient _certificateClient;
    private readonly IMemoryCache _memoryCache;
    private readonly SecretClient _secretClient;
    private readonly VaultOptions _vaultOptions;

    public VaultService(CertificateClient certificateClient, IMemoryCache memoryCache, SecretClient secretClient, IOptions<VaultOptions> vaultOptions)
    {
        _certificateClient = certificateClient;
        _memoryCache = memoryCache;
        _secretClient = secretClient;
        _vaultOptions = vaultOptions.Value;
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value;
    }

    public async Task<X509Certificate2> GetCertificateAsync(string certificateName)
    {
        var cacheKey = $"vault-cert::{certificateName.ToLowerInvariant()}";

        if (_memoryCache.TryGetValue<X509Certificate2>(cacheKey, out var cachedCertificate))
        {
            return cachedCertificate!;
        }

        return await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_vaultOptions.CertificateCacheMinutes);
            entry.RegisterPostEvictionCallback((key, _, reason, __) =>
            {
                Console.WriteLine($"Cache entry '{key}' evicted. Reason: {reason}.");
            });

            KeyVaultCertificateWithPolicy certificateWithPolicy = await _certificateClient.GetCertificateAsync(certificateName);
            KeyVaultSecret certificateSecret = await _secretClient.GetSecretAsync(certificateWithPolicy.Name, certificateWithPolicy.Properties.Version);

            if (string.Equals(certificateSecret.Properties.ContentType, "application/x-pkcs12", StringComparison.OrdinalIgnoreCase))
            {
                byte[] pfxBytes = Convert.FromBase64String(certificateSecret.Value);
                return X509CertificateLoader.LoadPkcs12(pfxBytes, password: null);
            }

            throw new InvalidOperationException($"Unsupported certificate content type '{certificateSecret.Properties.ContentType}' for certificate '{certificateName}'.");
        }) ?? throw new InvalidOperationException($"Certificate '{certificateName}' could not be loaded.");
    }
}