using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalTimelineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Rentals",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Rentals",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Rentals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Rentals",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rentals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Rentals",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RentalTimeline",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RentalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalTimeline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalTimeline_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CreatedAt",
                table: "Rentals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_IsDeleted",
                table: "Rentals",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RentalTimeline_RentalId",
                table: "RentalTimeline",
                column: "RentalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RentalTimeline");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_CreatedAt",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_IsDeleted",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Rentals");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "Rentals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Rentals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
