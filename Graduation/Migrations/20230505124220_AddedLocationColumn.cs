using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation.Migrations
{
    public partial class AddedLocationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                schema: "security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                schema: "security",
                table: "Users");
        }
    }
}
