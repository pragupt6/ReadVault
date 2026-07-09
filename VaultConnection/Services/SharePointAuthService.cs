using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using PnP.Core.Auth;
using PnP.Core.Services;
using Services;
using VaultConnection.Options;

namespace VaultConnection.Services;
public class SharePointAuthService: ISharePointAuthService
{
    private readonly IPnPContextFactory _pnpContextFactory;
    private readonly IVaultClient _vaultClient;
    private readonly SharePointOptions _sharePointOptions;

    public SharePointAuthService(IPnPContextFactory pnpContextFactory, IVaultClient vaultClient, IOptions<SharePointOptions> sharePointOptions)
    {
        _pnpContextFactory = pnpContextFactory;
        _vaultClient = vaultClient;
        _sharePointOptions = sharePointOptions.Value;
    }

    public async Task<X509Certificate2> GetSharePointCertificateAsync()
    {
        return await _vaultClient.GetCertificateAsync(_sharePointOptions.CertificateName);
    }

    public async Task<PnPContext> CreateSiteContextAsync(CancellationToken cancellationToken = default)
    {
        var certificate = await GetSharePointCertificateAsync();
        var siteUri = new Uri(_sharePointOptions.SiteUrl);
        var authProvider = new X509CertificateAuthenticationProvider(
            _sharePointOptions.ClientId,
            _sharePointOptions.TenantId,
            certificate);

        return await _pnpContextFactory.CreateAsync(siteUri, authProvider, cancellationToken);
    }

    public async Task<SharePointConnectionResult> ConnectToSiteAsync(CancellationToken cancellationToken = default)
    {
        using var context = await CreateSiteContextAsync(cancellationToken);
        await context.Web.LoadAsync(p => p.Title, p => p.Url);

        return new SharePointConnectionResult(
            IsConnected: true,
            SiteTitle: context.Web.Title,
            SiteUrl: context.Uri.ToString());
    }
}

