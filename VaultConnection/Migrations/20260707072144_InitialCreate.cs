using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VaultConnection.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharePointSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SiteUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharePointSites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharePointListDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ListTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SharePointSiteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharePointListDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharePointListDetails_SharePointSites_SharePointSiteId",
                        column: x => x.SharePointSiteId,
                        principalTable: "SharePointSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SharePointSites",
                columns: new[] { "Id", "SiteTitle", "SiteUrl" },
                values: new object[,]
                {
                    { 1, "The Organization Perspective", "https://mso365e5.sharepoint.com/sites/ThePerspective" },
                    { 2, "Sample SharePoint Site", "https://mso365e5.sharepoint.com/sites/SampleSharePointSite" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharePointListDetails_SharePointSiteId_ListId",
                table: "SharePointListDetails",
                columns: new[] { "SharePointSiteId", "ListId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharePointListDetails");

            migrationBuilder.DropTable(
                name: "SharePointSites");
        }
    }
}
