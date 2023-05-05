using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Graduation.Migrations
{
    public partial class AddedPropsToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "security",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "security",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "security",
                table: "Users");
        }
    }
}
