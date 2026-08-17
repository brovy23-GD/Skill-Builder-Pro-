using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentCompletionEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignmentCompletionEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    DrillId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingAttempts = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentCompletionEvents", x => x.Id);
                    table.CheckConstraint("CK_AssignmentCompletionEvents_EventType", "[EventType] = 'AssignmentRecipientCompleted'");
                    table.CheckConstraint("CK_AssignmentCompletionEvents_ProcessingAttempts", "[ProcessingAttempts] >= 0");
                    table.ForeignKey(
                        name: "FK_AssignmentCompletionEvents_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentCompletionEvents_DrillAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "DrillAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentCompletionEvents_Drills_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentCompletionEvents_AssignmentId_AthleteUserId_EventType",
                table: "AssignmentCompletionEvents",
                columns: new[] { "AssignmentId", "AthleteUserId", "EventType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentCompletionEvents_AthleteUserId",
                table: "AssignmentCompletionEvents",
                column: "AthleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentCompletionEvents_DrillId",
                table: "AssignmentCompletionEvents",
                column: "DrillId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentCompletionEvents_ProcessedAtUtc_CreatedAtUtc",
                table: "AssignmentCompletionEvents",
                columns: new[] { "ProcessedAtUtc", "CreatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentCompletionEvents");
        }
    }
}
