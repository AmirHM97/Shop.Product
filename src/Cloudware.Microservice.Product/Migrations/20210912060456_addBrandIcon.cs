using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class addBrandIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Stock",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ProductBrands",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ProductBrands");
        }
    }
}
