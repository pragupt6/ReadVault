using Microsoft.EntityFrameworkCore;
using VaultConnection.Models;

namespace VaultConnection.Data;

public sealed class VaultConnectionDbContext : DbContext
{
    public VaultConnectionDbContext(DbContextOptions<VaultConnectionDbContext> options)
        : base(options)
    {
    }

    public DbSet<SharePointSite> SharePointSites => Set<SharePointSite>();

    public DbSet<SharePointListDetail> SharePointListDetails => Set<SharePointListDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SharePointSite>(entity =>
        {
            entity.ToTable("SharePointSites");
            entity.HasKey(site => site.Id);
            entity.Property(site => site.SiteTitle).IsRequired().HasMaxLength(200);
            entity.Property(site => site.SiteUrl).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<SharePointListDetail>(entity =>
        {
            entity.ToTable("SharePointListDetails");
            entity.HasKey(list => list.Id);
            entity.Property(list => list.ListId).IsRequired().HasMaxLength(100);
            entity.Property(list => list.ListTitle).IsRequired().HasMaxLength(200);

            entity.HasOne(list => list.SharePointSite)
                .WithMany(site => site.ListDetails)
                .HasForeignKey(list => list.SharePointSiteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(list => new { list.SharePointSiteId, list.ListId }).IsUnique();
        });

        modelBuilder.Entity<SharePointSite>().HasData(
            new SharePointSite
            {
                Id = 1,
                SiteTitle = "The Organization Perspective",
                SiteUrl = "https://mso365e5.sharepoint.com/sites/ThePerspective"
            },
            new SharePointSite
            {
                Id = 2,
                SiteTitle = "Sample SharePoint Site",
                SiteUrl = "https://mso365e5.sharepoint.com/sites/SampleSharePointSite"
            });
    }
}