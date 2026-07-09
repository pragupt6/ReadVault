using System.ComponentModel.DataAnnotations;

namespace VaultConnection.Options;

public sealed class VaultOptions
{
    public const string SectionName = "Vault";

    [Required]
    public string KeyVaultUri { get; set; } = string.Empty;

    [Required]
    public string SecretName { get; set; } = string.Empty;

    [Required]
    public string CertificateName { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int CertificateCacheMinutes { get; set; } = 30;
}
