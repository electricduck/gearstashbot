using Microsoft.EntityFrameworkCore.Migrations;

namespace StashBot.Migrations
{
    public partial class Modify_Author_003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanRandomizeQueue",
                table: "Authors",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanRandomizeQueue",
                table: "Authors");
        }
    }
}
