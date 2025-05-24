using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArsWebApiService.Migrations.MyDbContext2Migrations
{
    public partial class _003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "StudentNew",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Age2",
                table: "StudentNew",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "StudentNew");

            migrationBuilder.DropColumn(
                name: "Age2",
                table: "StudentNew");
        }
    }
}
