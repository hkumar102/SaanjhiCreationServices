using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop and recreate Occasion column as text[]
            migrationBuilder.DropColumn(
                name: "Occasion",
                table: "Products");
            migrationBuilder.AddColumn<string[]>(
                name: "Occasion",
                table: "Products",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            // Drop and recreate AvailableSizes column as text[]
            migrationBuilder.DropColumn(
                name: "AvailableSizes",
                table: "Products");
            migrationBuilder.AddColumn<string[]>(
                name: "AvailableSizes",
                table: "Products",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            // Drop and recreate AvailableColors column as text[]
            migrationBuilder.DropColumn(
                name: "AvailableColors",
                table: "Products");
            migrationBuilder.AddColumn<string[]>(
                name: "AvailableColors",
                table: "Products",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Occasion",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "AvailableSizes",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "AvailableColors",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
