using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VaultConnection.Options;

namespace VaultConnection.Data;

public sealed class VaultConnectionDbContextFactory : IDesignTimeDbContextFactory<VaultConnectionDbContext>
{
    public VaultConnectionDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var sharePointOptions = configuration.GetSection(SharePointOptions.SectionName).Get<SharePointOptions>();
        var connectionString = sharePointOptions?.DefaultConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{SharePointOptions.SectionName}:DefaultConnectionString' was not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<VaultConnectionDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure());
        return new VaultConnectionDbContext(optionsBuilder.Options);
    }
}