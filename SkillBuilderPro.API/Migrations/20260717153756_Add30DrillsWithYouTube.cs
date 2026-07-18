using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillBuilderPro.API.Migrations
{
    /// <inheritdoc />
    public partial class Add30DrillsWithYouTube : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Improve handles with advanced dribbling techniques.", "Ball Handling Drills" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Shooting", "Master proper shooting technique and form.", "Shooting Form Basics", "Basketball", "https://www.youtube.com/watch?v=hYaLYGmS61A" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Defense", "Learn proper defensive stance and movement.", 2, "Defensive Footwork", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.InsertData(
                table: "Drills",
                columns: new[] { "Id", "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[,]
                {
                    { 4, "Passing", "Develop precision passing and court vision.", 1, "Passing Accuracy", "Basketball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 5, "Footwork", "Build explosive footwork and lateral agility.", 3, "Footwork Conditioning", "Basketball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 6, "Passing", "Master offensive route running patterns.", 2, "Passing Routes", "Football", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 7, "Defense", "Learn proper tackling form and safety.", 2, "Tackling Technique", "Football", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 8, "Blocking", "Develop offensive line blocking fundamentals.", 2, "Line Blocking", "Football", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 9, "Ball Handling", "Prevent fumbles with strong ball security drills.", 1, "Ball Security", "Football", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 10, "Conditioning", "Build speed and endurance for game performance.", 3, "Conditioning Sprints", "Football", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 11, "Hitting", "Perfect your stance and swing mechanics.", 1, "Batting Stance", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 12, "Running", "Improve speed and technique between bases.", 2, "Base Running", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 13, "Fielding", "Master ground ball and fly ball techniques.", 2, "Fielding Basics", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 14, "Pitching", "Learn proper pitching form and control.", 3, "Pitching Mechanics", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 15, "Throwing", "Develop arm strength and throwing precision.", 2, "Throwing Accuracy", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 16, "Hitting", "Tee placement and batting technique.", 2, "Softball Hitting Drills", "Softball", "https://www.youtube.com/watch?v=ZxkrWk5j2NE" },
                    { 17, "Defense", "Learn proper fielding positions and angles.", 1, "Defensive Positioning", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 18, "Running", "Master safe and effective sliding into bases.", 2, "Sliding Technique", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 19, "Throwing", "Build arm strength with proper throwing form.", 2, "Throwing Fundamentals", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 20, "Catching", "Improve catching technique and pop time.", 3, "Catching Skills", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 21, "Dribbling", "Enhance touch and close ball control.", 2, "Ball Control", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 22, "Passing", "Master short and long range passing.", 1, "Passing Accuracy", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 23, "Shooting", "Improve shot power and accuracy toward goal.", 2, "Shooting Technique", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 24, "Defense", "Learn positioning and marking techniques.", 2, "Defensive Tactics", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 25, "Ball Control", "Develop soft first touch and ball reception.", 1, "First Touch", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 26, "Dribbling", "Master puck control at speed.", 2, "Stick Handling", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 27, "Shooting", "Improve shot speed and placement.", 2, "Shooting Accuracy", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 28, "Defense", "Learn proper body checking form.", 3, "Checking Technique", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 29, "Passing", "Develop precision tape-to-tape passing.", 1, "Passing Drills", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" },
                    { 30, "Skating", "Build speed, balance, and edge work.", 3, "Skating Agility", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Two-ball pound dribbles, crossovers, figure 8s.", "Stationary Ball Handling" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Hitting", "Tee placement inside, focus on staying connected through the zone.", "Tee Work - Inside Pitch", "Softball", "https://www.youtube.com/watch?v=ZxkrWk5j2NE" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Shooting", "One-hand form shots from 5 spots inside the paint.", 1, "Form Shooting Close Range", "https://www.youtube.com/watch?v=hYaLYGmS61A" });
        }
    }
}
