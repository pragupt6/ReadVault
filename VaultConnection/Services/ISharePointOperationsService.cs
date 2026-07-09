using System.Security.Cryptography.X509Certificates;

namespace VaultConnection.Services;

public interface ISharePointOperationsService
{
    Task<SiteList[]> GetSiteListsAsync(CancellationToken cancellationToken = default);
}