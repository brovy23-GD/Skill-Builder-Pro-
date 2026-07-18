using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateYouTubeLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=UVAz2aASZx4");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=ZxkrWk5j2NE");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                column: "VideoUrl",
                value: "https://www.youtube.com/watch?v=hYaLYGmS61A");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
