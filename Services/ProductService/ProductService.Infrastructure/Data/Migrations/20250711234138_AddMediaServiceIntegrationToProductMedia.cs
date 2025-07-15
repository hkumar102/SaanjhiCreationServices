using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaServiceIntegrationToProductMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductMedia",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "ProductMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "ProductMedia",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "ProductMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "ProductMedia",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "ProductMedia",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "ProductMedia",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessingStatus",
                table: "ProductMedia",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "ProductMedia",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "ProductMedia",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VariantsJson",
                table: "ProductMedia",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "ProductMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "ProcessingStatus",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "VariantsJson",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ProductMedia");
        }
    }
}
