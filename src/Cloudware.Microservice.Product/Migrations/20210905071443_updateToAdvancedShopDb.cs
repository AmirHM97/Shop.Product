using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class updateToAdvancedShopDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockProperty_PropertyItems_PropItemId",
                table: "StockProperty");

            migrationBuilder.DropTable(
                name: "PropertyItems");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropIndex(
                name: "IX_StockProperty_PropItemId",
                table: "StockProperty");

            migrationBuilder.RenameColumn(
                name: "PropItemId",
                table: "StockProperty",
                newName: "PropertyId");

            migrationBuilder.AddColumn<int>(
                name: "PropertyType",
                table: "StockProperty",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PropertyCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyCategory_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCategory_CategoryId",
                table: "PropertyCategory",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyCategory");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "StockProperty");

            migrationBuilder.RenameColumn(
                name: "PropertyId",
                table: "StockProperty",
                newName: "PropItemId");

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductItemId = table.Column<long>(type: "bigint", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Property_Product_ProductItemId",
                        column: x => x.ProductItemId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Propid = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyItems_Property_Propid",
                        column: x => x.Propid,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockProperty_PropItemId",
                table: "StockProperty",
                column: "PropItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_ProductItemId",
                table: "Property",
                column: "ProductItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyItems_Propid",
                table: "PropertyItems",
                column: "Propid");

            migrationBuilder.AddForeignKey(
                name: "FK_StockProperty_PropertyItems_PropItemId",
                table: "StockProperty",
                column: "PropItemId",
                principalTable: "PropertyItems",
                principalColumn: "Id");
        }
    }
}
