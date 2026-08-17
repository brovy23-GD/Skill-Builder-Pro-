using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddAthleteProgressionFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AthleteProgressions",
                columns: table => new
                {
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    OverallRank = table.Column<int>(type: "int", nullable: false),
                    ProgressionScore = table.Column<int>(type: "int", nullable: false),
                    TotalQualifyingCompletions = table.Column<int>(type: "int", nullable: false),
                    ActiveSkillCount = table.Column<int>(type: "int", nullable: false),
                    CurrentOverallStreak = table.Column<int>(type: "int", nullable: false),
                    LongestOverallStreak = table.Column<int>(type: "int", nullable: false),
                    LastCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgressToNextRank = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteProgressions", x => x.AthleteUserId);
                    table.CheckConstraint("CK_AthleteProgressions_NonNegative", "[ProgressionScore] >= 0 AND [TotalQualifyingCompletions] >= 0 AND [ActiveSkillCount] >= 0 AND [CurrentOverallStreak] >= 0 AND [LongestOverallStreak] >= 0");
                    table.CheckConstraint("CK_AthleteProgressions_Percent", "[ProgressToNextRank] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_AthleteProgressions_Rank", "[OverallRank] BETWEEN 1 AND 8");
                    table.ForeignKey(
                        name: "FK_AthleteProgressions_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AthleteSkillProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AthleteUserId = table.Column<int>(type: "int", nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    QualifyingCompletions = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: true),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false),
                    LongestStreak = table.Column<int>(type: "int", nullable: false),
                    LastCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgressToNextLevel = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleteSkillProgress", x => x.Id);
                    table.CheckConstraint("CK_AthleteSkillProgress_Level", "[CurrentLevel] BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_AthleteSkillProgress_NonNegative", "[QualifyingCompletions] >= 0 AND [CurrentStreak] >= 0 AND [LongestStreak] >= 0");
                    table.CheckConstraint("CK_AthleteSkillProgress_Percent", "[ProgressToNextLevel] BETWEEN 0 AND 100");
                    table.CheckConstraint("CK_AthleteSkillProgress_Rating", "[AverageRating] IS NULL OR ([AverageRating] >= 1 AND [AverageRating] <= 5)");
                    table.ForeignKey(
                        name: "FK_AthleteSkillProgress_AspNetUsers_AthleteUserId",
                        column: x => x.AthleteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleteSkillProgress_AthleteUserId_Sport_Category_SubCategory",
                table: "AthleteSkillProgress",
                columns: new[] { "AthleteUserId", "Sport", "Category", "SubCategory" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleteProgressions");

            migrationBuilder.DropTable(
                name: "AthleteSkillProgress");
        }
    }
}
