using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GearstashBot.Migrations
{
    public partial class Modify_Author_004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                table: "Authors",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TelegramLanguage",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "TelegramLanguage",
                table: "Authors");
        }
    }
}
