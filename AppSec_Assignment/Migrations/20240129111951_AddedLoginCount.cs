using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppSec_Assignment.Migrations
{
    public partial class AddedLoginCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Logs",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logs",
                table: "AspNetUsers");
        }
    }
}
