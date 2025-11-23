using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixHiLoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameSequence(
                name: "catalog_hilo",
                newName: "catalog_hilo",
                newSchema: "catalog");

            migrationBuilder.RenameSequence(
                name: "catalog_brand_hilo",
                newName: "catalog_brand_hilo",
                newSchema: "catalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameSequence(
                name: "catalog_hilo",
                schema: "catalog",
                newName: "catalog_hilo");

            migrationBuilder.RenameSequence(
                name: "catalog_brand_hilo",
                schema: "catalog",
                newName: "catalog_brand_hilo");
        }
    }
}
