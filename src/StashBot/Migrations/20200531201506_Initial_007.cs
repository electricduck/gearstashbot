using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StashBot.Migrations
{
    public partial class Initial_007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TelegramId = table.Column<long>(nullable: false),
                    CanDeleteOthers = table.Column<bool>(nullable: false),
                    CanFlushQueue = table.Column<bool>(nullable: false),
                    CanManageAuthors = table.Column<bool>(nullable: false),
                    CanQueue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Queue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MediaUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SourceName = table.Column<string>(nullable: true),
                    SourceUrl = table.Column<string>(nullable: true),
                    UsernameUrl = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    PostedAt = table.Column<DateTime>(nullable: false),
                    QueuedAt = table.Column<DateTime>(nullable: false),
                    AuthorTelegramId = table.Column<int>(nullable: false),
                    AuthorTelegramUsername = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Queue");
        }
    }
}
