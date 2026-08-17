using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDrillAssignmentsAndRecipients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrillAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrillId = table.Column<int>(type: "int", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "int", nullable: false),
                    SourceTeamId = table.Column<int>(type: "int", nullable: true),
                    ScheduledForUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountsTowardProgression = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillAssignments", x => x.Id);
                    table.CheckConstraint("CK_DrillAssignments_Status", "[Status] IN ('Scheduled', 'Active', 'Cancelled', 'Closed')");
                    table.ForeignKey(
                        name: "FK_DrillAssignments_AspNetUsers_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DrillAssignments_Drills_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drills",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DrillAssignments_Teams_SourceTeamId",
                        column: x => x.SourceTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DrillAssignmentRecipients",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AthleteNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrillAssignmentRecipients", x => new { x.AssignmentId, x.AthleteUserId });
                    table.CheckConstraint("CK_DrillAssignmentRecipients_Rating", "[Rating] IS NULL OR ([Rating] >= 1 AND [Rating] <= 5)");
                    table.CheckConstraint("CK_DrillAssignmentRecipients_Status", "[Status] IN ('Assigned', 'InProgress', 'Completed', 'Missed', 'Excused')");
                    table.ForeignKey(
                        name: "FK_DrillAssignmentRecipients_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DrillAssignmentRecipients_DrillAssignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "DrillAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrillAssignmentRecipients_AthleteUserId_CompletedAtUtc",
                table: "DrillAssignmentRecipients",
                columns: new[] { "AthleteUserId", "CompletedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DrillAssignmentRecipients_AthleteUserId_Status_AssignmentId",
                table: "DrillAssignmentRecipients",
                columns: new[] { "AthleteUserId", "Status", "AssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_DrillAssignments_AssignedByUserId_CreatedAtUtc",
                table: "DrillAssignments",
                columns: new[] { "AssignedByUserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DrillAssignments_DrillId",
                table: "DrillAssignments",
                column: "DrillId");

            migrationBuilder.CreateIndex(
                name: "IX_DrillAssignments_SourceTeamId_ScheduledForUtc",
                table: "DrillAssignments",
                columns: new[] { "SourceTeamId", "ScheduledForUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrillAssignmentRecipients");

            migrationBuilder.DropTable(
                name: "DrillAssignments");
        }
    }
}
