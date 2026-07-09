namespace VaultConnection.Models;

public sealed class SharePointListDetail
{
    public int Id { get; set; }

    public string ListId { get; set; } = string.Empty;

    public string ListTitle { get; set; } = string.Empty;

    public int SharePointSiteId { get; set; }

    public SharePointSite? SharePointSite { get; set; }
}