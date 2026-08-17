using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletionEventProgressLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "ProgressLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "AssignmentCompletionEventId",
                table: "ProgressLogs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_AssignmentCompletionEventId",
                table: "ProgressLogs",
                column: "AssignmentCompletionEventId",
                unique: true,
                filter: "[AssignmentCompletionEventId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressLogs_AssignmentCompletionEvents_AssignmentCompletionEventId",
                table: "ProgressLogs",
                column: "AssignmentCompletionEventId",
                principalTable: "AssignmentCompletionEvents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressLogs_AssignmentCompletionEvents_AssignmentCompletionEventId",
                table: "ProgressLogs");

            migrationBuilder.DropIndex(
                name: "IX_ProgressLogs_AssignmentCompletionEventId",
                table: "ProgressLogs");

            migrationBuilder.DropColumn(
                name: "AssignmentCompletionEventId",
                table: "ProgressLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "ProgressLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
