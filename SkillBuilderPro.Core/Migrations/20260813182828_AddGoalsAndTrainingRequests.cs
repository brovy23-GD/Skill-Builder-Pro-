using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalsAndTrainingRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AthleteGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedByRole = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GoalType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetValue = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteGoals", x => x.Id);
                    table.CheckConstraint("CK_AthleteGoals_GoalType", "[GoalType] IN ('QualifyingCompletions','SkillLevel','OverallRank','TrainingStreak')");
                    table.CheckConstraint("CK_AthleteGoals_Status", "[Status] IN ('Active','Completed','Cancelled')");
                    table.CheckConstraint("CK_AthleteGoals_TargetValue", "[TargetValue] > 0");
                    table.ForeignKey(
                        name: "FK_AthleteGoals_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AthleteGoals_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    RequestedRecipientUserId = table.Column<int>(type: "int", nullable: false),
                    RequestedRecipientRole = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    RequestedDrillId = table.Column<int>(type: "int", nullable: true),
                    Sport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAssignmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRequests", x => x.Id);
                    table.CheckConstraint("CK_TrainingRequests_RecipientRole", "[RequestedRecipientRole] IN ('Parent','Coach')");
                    table.CheckConstraint("CK_TrainingRequests_Status", "[Status] IN ('Pending','Approved','Declined','Cancelled')");
                    table.ForeignKey(
                        name: "FK_TrainingRequests_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingRequests_AspNetUsers_RequestedRecipientUserId",
                        column: x => x.RequestedRecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingRequests_DrillAssignments_ApprovedAssignmentId",
                        column: x => x.ApprovedAssignmentId,
                        principalTable: "DrillAssignments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingRequests_Drills_RequestedDrillId",
                        column: x => x.RequestedDrillId,
                        principalTable: "Drills",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrainingRequests_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteGoals_AthleteUserId_Status_CreatedAtUtc",
                table: "AthleteGoals",
                columns: new[] { "AthleteUserId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteGoals_CreatedByUserId",
                table: "AthleteGoals",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_ApprovedAssignmentId",
                table: "TrainingRequests",
                column: "ApprovedAssignmentId",
                unique: true,
                filter: "[ApprovedAssignmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_AthleteUserId_Status_CreatedAtUtc",
                table: "TrainingRequests",
                columns: new[] { "AthleteUserId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_RequestedDrillId",
                table: "TrainingRequests",
                column: "RequestedDrillId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_RequestedRecipientUserId_Status_CreatedAtUtc",
                table: "TrainingRequests",
                columns: new[] { "RequestedRecipientUserId", "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRequests_TeamId",
                table: "TrainingRequests",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteGoals");

            migrationBuilder.DropTable(
                name: "TrainingRequests");
        }
    }
}
