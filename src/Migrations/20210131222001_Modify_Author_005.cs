using Microsoft.EntityFrameworkCore.Migrations;

namespace GearstashBot.Migrations
{
    public partial class Modify_Author_005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAnnounce",
                table: "Authors",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAnnounce",
                table: "Authors");
        }
    }
}
