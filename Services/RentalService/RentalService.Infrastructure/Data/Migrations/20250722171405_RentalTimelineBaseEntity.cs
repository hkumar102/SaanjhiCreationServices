using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RentalTimelineBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedByUserId",
                table: "RentalTimeline");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RentalTimeline",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RentalTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RentalTimeline",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RentalTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RentalTimeline",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "RentalTimeline",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RentalTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RentalTimeline",
                type: "bytea",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RentalTimeline");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RentalTimeline");

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedByUserId",
                table: "RentalTimeline",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
