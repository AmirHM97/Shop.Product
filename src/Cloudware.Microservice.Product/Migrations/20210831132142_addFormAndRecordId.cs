using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class addFormAndRecordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormId",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordId",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalPropertyFormId",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalPropertyRecordId",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "TechnicalPropertyFormId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "TechnicalPropertyRecordId",
                table: "Product");
        }
    }
}
