using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class updateDateTimeOffsetandTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalProperty_Product_productItemId",
                table: "TechnicalProperty");

            migrationBuilder.RenameColumn(
                name: "productItemId",
                table: "TechnicalProperty",
                newName: "ProductItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TechnicalProperty_productItemId",
                table: "TechnicalProperty",
                newName: "IX_TechnicalProperty_ProductItemId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "TechnicalProperty",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "TechnicalProperty",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "Stock",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Stock",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "ProductBrands",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "ProductBrands",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastUpdatedDate",
                table: "Product",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Product",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalProperty_Product_ProductItemId",
                table: "TechnicalProperty",
                column: "ProductItemId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechnicalProperty_Product_ProductItemId",
                table: "TechnicalProperty");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TechnicalProperty");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "TechnicalProperty");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ProductBrands");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "ProductBrands");

            migrationBuilder.RenameColumn(
                name: "ProductItemId",
                table: "TechnicalProperty",
                newName: "productItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TechnicalProperty_ProductItemId",
                table: "TechnicalProperty",
                newName: "IX_TechnicalProperty_productItemId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Stock",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Stock",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Product",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Product",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddForeignKey(
                name: "FK_TechnicalProperty_Product_productItemId",
                table: "TechnicalProperty",
                column: "productItemId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
