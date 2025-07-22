using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RentalChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualReturnDate",
                table: "Rentals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyRate",
                table: "Rentals",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DamageFee",
                table: "Rentals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "Rentals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "LateFee",
                table: "Rentals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnConditionNotes",
                table: "Rentals",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualReturnDate",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DailyRate",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DamageFee",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "LateFee",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "ReturnConditionNotes",
                table: "Rentals");
        }
    }
}
