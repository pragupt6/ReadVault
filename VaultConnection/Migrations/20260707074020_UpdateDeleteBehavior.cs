using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultConnection.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharePointListDetails_SharePointSites_SharePointSiteId",
                table: "SharePointListDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_SharePointListDetails_SharePointSites_SharePointSiteId",
                table: "SharePointListDetails",
                column: "SharePointSiteId",
                principalTable: "SharePointSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharePointListDetails_SharePointSites_SharePointSiteId",
                table: "SharePointListDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_SharePointListDetails_SharePointSites_SharePointSiteId",
                table: "SharePointListDetails",
                column: "SharePointSiteId",
                principalTable: "SharePointSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
