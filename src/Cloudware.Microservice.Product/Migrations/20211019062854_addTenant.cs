using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class addTenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Guarantees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Guarantees");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Colors");
        }
    }
}
