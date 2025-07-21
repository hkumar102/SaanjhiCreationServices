using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQRCodeImageBase64ToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeImageBase64",
                table: "InventoryItems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeImageBase64",
                table: "InventoryItems");
        }
    }
}
