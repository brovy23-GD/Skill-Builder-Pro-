using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDrillExternalSourceKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSourceKey",
                table: "Drills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drills_ExternalSourceKey",
                table: "Drills",
                column: "ExternalSourceKey",
                unique: true,
                filter: "[ExternalSourceKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drills_ExternalSourceKey",
                table: "Drills");

            migrationBuilder.DropColumn(
                name: "ExternalSourceKey",
                table: "Drills");
        }
    }
}
