namespace VaultConnection.Models;

public sealed class SharePointSite
{
    public int Id { get; set; }

    public string SiteTitle { get; set; } = string.Empty;

    public string SiteUrl { get; set; } = string.Empty;

    public ICollection<SharePointListDetail> ListDetails { get; set; } = new List<SharePointListDetail>();
}