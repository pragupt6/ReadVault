using System.ComponentModel.DataAnnotations;
namespace VaultConnection.Options;
public class SharePointOptions
{
    public const string SectionName = "SharePoint";

    [Required]
    public string SiteUrl { get; set; } = string.Empty;

    [Required]
    public string ClientId { get; set; } = string.Empty;

    [Required]
    public string TenantId { get; set; } = string.Empty;

    [Required]
    public string CertificateName { get; set; } = string.Empty;

    [Required]
    public string DefaultConnectionString { get; set; } = string.Empty;
}