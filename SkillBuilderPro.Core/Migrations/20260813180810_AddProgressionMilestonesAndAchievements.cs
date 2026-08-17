using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddProgressionMilestonesAndAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Tier = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementDefinitions", x => x.Id);
                    table.CheckConstraint("CK_AchievementDefinitions_SortOrder", "[SortOrder] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "AthleteRankHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    RankNumber = table.Column<int>(type: "int", nullable: false),
                    ProgressionScoreAtEarned = table.Column<int>(type: "int", nullable: false),
                    TotalQualifyingCompletionsAtEarned = table.Column<int>(type: "int", nullable: false),
                    ActiveSkillCountAtEarned = table.Column<int>(type: "int", nullable: false),
                    CurrentStreakAtEarned = table.Column<int>(type: "int", nullable: false),
                    EarnedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteRankHistories", x => x.Id);
                    table.CheckConstraint("CK_AthleteRankHistories_Rank", "[RankNumber] BETWEEN 2 AND 8");
                    table.ForeignKey(
                        name: "FK_AthleteRankHistories_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AthleteSkillLevelHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    QualifyingCompletionsAtEarned = table.Column<int>(type: "int", nullable: false),
                    AverageRatingAtEarned = table.Column<double>(type: "float", nullable: true),
                    EarnedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteSkillLevelHistories", x => x.Id);
                    table.CheckConstraint("CK_AthleteSkillLevelHistories_Level", "[Level] BETWEEN 2 AND 5");
                    table.ForeignKey(
                        name: "FK_AthleteSkillLevelHistories_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AthleteAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    AchievementDefinitionId = table.Column<int>(type: "int", nullable: false),
                    EarnedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SourceKey = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleteAchievements_AchievementDefinitions_AchievementDefinitionId",
                        column: x => x.AchievementDefinitionId,
                        principalTable: "AchievementDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AthleteAchievements_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementDefinitions_Code",
                table: "AchievementDefinitions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AchievementDefinitionId",
                table: "AthleteAchievements",
                column: "AchievementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AthleteUserId_AchievementDefinitionId",
                table: "AthleteAchievements",
                columns: new[] { "AthleteUserId", "AchievementDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteAchievements_AthleteUserId_EarnedAtUtc",
                table: "AthleteAchievements",
                columns: new[] { "AthleteUserId", "EarnedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteRankHistories_AthleteUserId_EarnedAtUtc",
                table: "AthleteRankHistories",
                columns: new[] { "AthleteUserId", "EarnedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteRankHistories_AthleteUserId_RankNumber",
                table: "AthleteRankHistories",
                columns: new[] { "AthleteUserId", "RankNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSkillLevelHistories_AthleteUserId_EarnedAtUtc",
                table: "AthleteSkillLevelHistories",
                columns: new[] { "AthleteUserId", "EarnedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSkillLevelHistories_AthleteUserId_Sport_Category_SubCategory_Level",
                table: "AthleteSkillLevelHistories",
                columns: new[] { "AthleteUserId", "Sport", "Category", "SubCategory", "Level" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteAchievements");

            migrationBuilder.DropTable(
                name: "AthleteRankHistories");

            migrationBuilder.DropTable(
                name: "AthleteSkillLevelHistories");

            migrationBuilder.DropTable(
                name: "AchievementDefinitions");
        }
    }
}
