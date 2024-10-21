using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GE.Integration.Shopee.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderImports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerPlanId = table.Column<long>(type: "bigint", nullable: true),
                    OrderId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EcommerceNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SalesChannel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductImports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerPlanId = table.Column<long>(type: "bigint", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderImports");

            migrationBuilder.DropTable(
                name: "ProductImports");
        }
    }
}
