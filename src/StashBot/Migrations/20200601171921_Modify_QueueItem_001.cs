using Microsoft.EntityFrameworkCore.Migrations;

namespace StashBot.Migrations
{
    public partial class Modify_QueueItem_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorTelegramName",
                table: "Queue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorTelegramName",
                table: "Queue");
        }
    }
}
