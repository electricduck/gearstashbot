using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GearstashBot.Migrations
{
    public partial class Initial_009 : Migration
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
                    TelegramName = table.Column<string>(nullable: true),
                    TelegramUsername = table.Column<string>(nullable: true),
                    CanDeleteOthers = table.Column<bool>(nullable: false),
                    CanFlushQueue = table.Column<bool>(nullable: false),
                    CanManageAuthors = table.Column<bool>(nullable: false),
                    CanQueue = table.Column<bool>(nullable: false),
                    TelegramDetailsLastUpdatedAt = table.Column<DateTime>(nullable: false)
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
                    AuthorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queue_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Queue_AuthorId",
                table: "Queue",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Queue");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
