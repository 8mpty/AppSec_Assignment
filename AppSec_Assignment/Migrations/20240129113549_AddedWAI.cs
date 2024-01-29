using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppSec_Assignment.Migrations
{
    public partial class AddedWAI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WAI",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WAI",
                table: "AspNetUsers");
        }
    }
}
