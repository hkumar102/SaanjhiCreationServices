using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddressToCustomerAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "CustomerAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_IsDeleted",
                table: "CustomerAddresses",
                newName: "IX_CustomerAddresses_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CustomerId",
                table: "CustomerAddresses",
                newName: "IX_CustomerAddresses_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_CreatedAt",
                table: "CustomerAddresses",
                newName: "IX_CustomerAddresses_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAddresses_Customers_CustomerId",
                table: "CustomerAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerAddresses",
                table: "CustomerAddresses");

            migrationBuilder.RenameTable(
                name: "CustomerAddresses",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerAddresses_IsDeleted",
                table: "Addresses",
                newName: "IX_Addresses_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "Addresses",
                newName: "IX_Addresses_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerAddresses_CreatedAt",
                table: "Addresses",
                newName: "IX_Addresses_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Customers_CustomerId",
                table: "Addresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
