using System;
using Microsoft.EntityFrameworkCore.Migrations;
using OlMag.Manufacture2.Models.Entities.SaleManager;

#nullable disable

namespace OlMag.Manufacture2.Migrations.SaleManagement
{
    /// <inheritdoc />
    public partial class AddCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SaleManagement");

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "SaleManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Information = table.Column<CustomerInformationEntity>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers",
                schema: "SaleManagement");
        }
    }
}
