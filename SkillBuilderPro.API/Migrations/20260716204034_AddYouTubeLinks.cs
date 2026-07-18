using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.API.Migrations
{
    /// <inheritdoc />
    public partial class AddYouTubeLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=JtHkGf0tLxk");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=QmQJvYwGq9U");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=QwGx0QmYt9w");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                column: "VideoUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                column: "VideoUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                column: "VideoUrl",
                value: "");
        }
    }
}
