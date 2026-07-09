using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services;
using VaultConnection;
using VaultConnection.Data;
using VaultConnection.Options;
using VaultConnection.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddVaultConnection(context.Configuration);
    })
    .Build();

using var scope = host.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<VaultConnectionDbContext>();
await dbContext.Database.MigrateAsync();

var vaultService = scope.ServiceProvider.GetRequiredService<IVaultClient>();
var vaultOptions = scope.ServiceProvider.GetRequiredService<IOptions<VaultOptions>>().Value;
var sharePointOptions = scope.ServiceProvider.GetRequiredService<IOptions<SharePointOptions>>().Value;

var sharePointOperationsService = scope.ServiceProvider.GetRequiredService<ISharePointOperationsService>();
var siteListsResult = await sharePointOperationsService.GetSiteListsAsync();
Console.WriteLine("\nSharePoint Site Lists:");
foreach (var list in siteListsResult)
{
    Console.WriteLine($"- {list.Title} (ID: {list.Id}, URL: {list.Url})");
}