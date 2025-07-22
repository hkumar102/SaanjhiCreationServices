using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RentalAddedSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"ALTER TABLE ""Rentals"" ALTER COLUMN ""Status"" TYPE integer USING ""Status""::integer;");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Rentals",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartDate",
                table: "Rentals",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualStartDate",
                table: "Rentals");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Rentals",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
