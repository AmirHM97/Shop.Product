using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cloudware.Microservice.Product.Migrations
{
    public partial class guaranteeDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Guarantees");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Guarantees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Guarantees");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "Guarantees",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
