using PnP.Core.Services;
using Microsoft.EntityFrameworkCore;
using VaultConnection.Data;
using VaultConnection.Models;

namespace VaultConnection.Services;
public class SharePointOperationsService: ISharePointOperationsService
{
    private readonly ISharePointAuthService _sharePointAuthService;
    private readonly VaultConnectionDbContext _dbContext;

    public SharePointOperationsService(ISharePointAuthService sharePointAuthService, VaultConnectionDbContext dbContext)
    {
        _sharePointAuthService = sharePointAuthService;
        _dbContext = dbContext;
    }

    private async Task<SharePointConnectionResult> ConnectToSiteAsync(CancellationToken cancellationToken = default)
    {
        return await _sharePointAuthService.ConnectToSiteAsync(cancellationToken);
    }

    public async Task<SiteList[]> GetSiteListsAsync(CancellationToken cancellationToken = default)
    {
        var connectionResult = await ConnectToSiteAsync(cancellationToken);

        if (!connectionResult.IsConnected)
        {
            throw new InvalidOperationException("Failed to connect to SharePoint site.");
        }

        using var context = await _sharePointAuthService.CreateSiteContextAsync(cancellationToken);
        await context.Web.Lists.LoadAsync(p => p.Title, p => p.Id, p => p.Hidden);

        var currentSite = await _dbContext.SharePointSites
            .FirstOrDefaultAsync(site => site.SiteUrl == connectionResult.SiteUrl, cancellationToken);

        if (currentSite is null)
        {
            currentSite = new SharePointSite
            {
                SiteTitle = connectionResult.SiteTitle,
                SiteUrl = connectionResult.SiteUrl
            };

            _dbContext.SharePointSites.Add(currentSite);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else if (!string.Equals(currentSite.SiteTitle, connectionResult.SiteTitle, StringComparison.Ordinal))
        {
            currentSite.SiteTitle = connectionResult.SiteTitle;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var existingDetails = await _dbContext.SharePointListDetails
            .Where(list => list.SharePointSiteId == currentSite.Id)
            .ToListAsync(cancellationToken);

        var existingByListId = existingDetails.ToDictionary(list => list.ListId, StringComparer.OrdinalIgnoreCase);
        var fetchedListIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var siteLists = new List<SiteList>();
        foreach (var list in context.Web.Lists)
        {
            var listId = list.Id.ToString();
            fetchedListIds.Add(listId);
            siteLists.Add(new SiteList(list.Title, listId, list.Title));

            if (existingByListId.TryGetValue(listId, out var existingDetail))
            {
                existingDetail.ListTitle = list.Title;
            }
            else
            {
                _dbContext.SharePointListDetails.Add(new SharePointListDetail
                {
                    ListId = listId,
                    ListTitle = list.Title,
                    SharePointSiteId = currentSite.Id
                });
            }
        }

        var staleDetails = existingDetails
            .Where(detail => !fetchedListIds.Contains(detail.ListId))
            .ToList();

        _dbContext.SharePointListDetails.RemoveRange(staleDetails);
        await _dbContext.SaveChangesAsync(cancellationToken);

        Console.WriteLine($"Total lists found: {siteLists.Count}");
        foreach (var list in siteLists)
        {
            Console.WriteLine($"- {list.Title}");
        }


        return siteLists.ToArray();
    }
}