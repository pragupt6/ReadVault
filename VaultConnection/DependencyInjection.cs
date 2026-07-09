using System;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PnP.Core.Services.Builder.Configuration;
using VaultConnection.Data;
using Services;
using VaultConnection.Options;
using VaultConnection.Services;
namespace VaultConnection;

public static class DependencyInjection
{
    public static IServiceCollection AddVaultConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<VaultOptions>()
            .Bind(configuration.GetSection(VaultOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => Uri.TryCreate(options.KeyVaultUri, UriKind.Absolute, out _), "Vault:KeyVaultUri must be a valid absolute URI.")
            .ValidateOnStart();
        services.AddOptions<SharePointOptions>()
            .Bind(configuration.GetSection(SharePointOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<VaultConnectionDbContext>(options =>
        {
            var sharePointOptions = configuration.GetSection(SharePointOptions.SectionName).Get<SharePointOptions>();
            var connectionString = sharePointOptions?.DefaultConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{SharePointOptions.SectionName}:DefaultConnectionString' was not found.");
            }

            options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
        });

        services.AddPnPCore();
        services.Configure<PnPCoreOptions>(options =>
        {
            options.PnPContext.GraphFirst = false;
        });

        services.AddMemoryCache();
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<VaultOptions>>().Value;
            var credential = new DefaultAzureCredential();
            return new SecretClient(new Uri(options.KeyVaultUri), credential);
        });
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<VaultOptions>>().Value;
            var credential = new DefaultAzureCredential();
            return new CertificateClient(new Uri(options.KeyVaultUri), credential);
        });
        services.AddSingleton<IVaultClient, VaultService>();
        services.AddSingleton<ISharePointAuthService, SharePointAuthService>();
        services.AddScoped<ISharePointOperationsService, SharePointOperationsService>();
        return services;
    }
}
