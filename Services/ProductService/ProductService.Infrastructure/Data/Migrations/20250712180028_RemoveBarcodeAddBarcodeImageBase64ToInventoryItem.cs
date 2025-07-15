using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBarcodeAddBarcodeImageBase64ToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_Barcode",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "InventoryItems");

            migrationBuilder.AddColumn<string>(
                name: "BarcodeImageBase64",
                table: "InventoryItems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeImageBase64",
                table: "InventoryItems");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "InventoryItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_Barcode",
                table: "InventoryItems",
                column: "Barcode");
        }
    }
}
