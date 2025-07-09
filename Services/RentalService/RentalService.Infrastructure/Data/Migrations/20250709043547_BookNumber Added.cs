using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookNumberAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookNumber",
                table: "Rentals",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookNumber",
                table: "Rentals");
        }
    }
}
