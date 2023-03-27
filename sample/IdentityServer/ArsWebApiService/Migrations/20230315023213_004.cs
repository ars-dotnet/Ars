using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApiWithIdentityServer4.Migrations
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "AppVersion");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppVersion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreationUserId",
                table: "AppVersion",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppVersion");

            migrationBuilder.DropColumn(
                name: "CreationUserId",
                table: "AppVersion");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "AppVersion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
