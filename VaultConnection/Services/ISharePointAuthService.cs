using System.Security.Cryptography.X509Certificates;
using PnP.Core.Services;

public interface ISharePointAuthService
{
    Task<X509Certificate2> GetSharePointCertificateAsync();
    Task<PnPContext> CreateSiteContextAsync(CancellationToken cancellationToken = default);
    Task<SharePointConnectionResult> ConnectToSiteAsync(CancellationToken cancellationToken = default);
}

public sealed record SharePointConnectionResult(
    bool IsConnected,
    string SiteTitle,
    string SiteUrl);