using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressAndScheduleOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "ProgressLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_OwnerUserId",
                table: "Schedules",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_OwnerUserId",
                table: "ProgressLogs",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressLogs_AspNetUsers_OwnerUserId",
                table: "ProgressLogs",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_AspNetUsers_OwnerUserId",
                table: "Schedules",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressLogs_AspNetUsers_OwnerUserId",
                table: "ProgressLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_AspNetUsers_OwnerUserId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_OwnerUserId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_ProgressLogs_OwnerUserId",
                table: "ProgressLogs");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "ProgressLogs");
        }
    }
}
