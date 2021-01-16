using Microsoft.EntityFrameworkCore.Migrations;

namespace GearstashBot.Migrations
{
    public partial class Index_QueueItem_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Queue_MessageId_SourceUrl",
                table: "Queue",
                columns: new[] { "MessageId", "SourceUrl" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Queue_MessageId_SourceUrl",
                table: "Queue");
        }
    }
}
