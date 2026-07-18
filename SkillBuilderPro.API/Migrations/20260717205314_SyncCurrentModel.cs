using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncCurrentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "5-minute dribbling workout that changes your game.", "https://www.youtube.com/watch?v=oADaM2L1YLc" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "Find your perfect shooting form.", "https://www.youtube.com/watch?v=x7anDE7OEww" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "Three defense drills to make your team better.", "https://www.youtube.com/watch?v=lFY__uSOJIY" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Rebounding", "Three best basketball rebounding drills that win games.", "Rebounding Techniques", "https://www.youtube.com/watch?v=pFRlEOeWpKY" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Passing", "Three basketball drills to become better at passing.", 1, "Passing Accuracy", "https://www.youtube.com/watch?v=OUskjh1r4Aw" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name", "VideoUrl" },
                values: new object[] { "How to throw a football with Tom Brady.", "Passing Technique", "https://www.youtube.com/watch?v=lv5p2Xqkxyk" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Catching", "WR drills with Odell Beckham Jr.", "Catching Skills", "https://www.youtube.com/watch?v=4n-Js1SwC2c" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Route Running", "Cooper Kupp's WR drills for creating separation.", "Route Running", "https://www.youtube.com/watch?v=b8Y-BrxoGQc" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Blocking", "Proper technique for run and pass blocking.", 2, "Blocking Fundamentals", "https://www.youtube.com/watch?v=hHyjR__k3XA" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name", "VideoUrl" },
                values: new object[] { "Ten speed and agility ladder drills.", "Speed and Agility", "https://www.youtube.com/watch?v=9ZTRUVLjGzI" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Ten best softball hitting drills for kids.", "Hitting Drills", "Softball", "https://www.youtube.com/watch?v=g-yDDzQL6eE" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Pitching", "Basic five steps for a beginner pitcher.", "Pitching Mechanics", "Softball", "https://www.youtube.com/watch?v=mIx9CvpGXsU" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Three infield drills for youth players.", 1, "Infield Drills", "Softball", "https://www.youtube.com/watch?v=6z0cpY5nGMA" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Fielding", "Must-do outfield drills with Gold Glover AJ Andrews.", 2, "Outfield Skills", "Softball", "https://www.youtube.com/watch?v=QREFQP72W0U" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Catching", "How to improve as a softball catcher.", 3, "Catcher Fundamentals", "Softball", "https://www.youtube.com/watch?v=qwdeRteH3es" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Ten best baseball hitting drills for kids.", 1, "Hitting Drills", "Baseball", "https://www.youtube.com/watch?v=gOE484Meo_o" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Pitching", "Must-do youth baseball pitching drills.", 2, "Pitching Drills", "Baseball", "https://www.youtube.com/watch?v=McHb2hXrTrE" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Fielding", "The top four infield drills.", 1, "Infield Drills", "Baseball", "https://www.youtube.com/watch?v=Uj5lw17XvuI" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Fielding", "Baseball outfield drills you must be doing.", "Outfield Drills", "Baseball", "https://www.youtube.com/watch?v=WUIM8NqNETg" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "How to become a better baseball catcher.", "Catcher Training", "Baseball", "https://www.youtube.com/watch?v=KJZHdoPxvW0" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Skating", "Edge-work drills from level 1 to 100.", "Edge-Work Skating", "Hockey", "https://www.youtube.com/watch?v=pp0Y3BDDp4A" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Dribbling", "Five-minute daily stickhandling routine.", 2, "Stickhandling Routine", "Hockey", "https://www.youtube.com/watch?v=7HluVwbAv3w" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Fifteen hockey shooting drills.", "Shooting Drills", "Hockey", "https://www.youtube.com/watch?v=RrYFNdTNvkc" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Passing", "How to catch and receive passes.", 1, "Passing Technique", "Hockey", "https://www.youtube.com/watch?v=BFI7jzMgu6Q" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Defense", "How to play better defense in hockey.", 2, "Defensive Positioning", "Hockey", "https://www.youtube.com/watch?v=HkNAK40ugkw" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Five essential dribbling drills.", 1, "Dribbling Drills", "Soccer", "https://www.youtube.com/watch?v=jwIHc9rz7yo" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Ten finishing exercises to become clinical.", "Finishing Exercises", "Soccer", "https://www.youtube.com/watch?v=0u8kPwXXsLA" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Passing", "Ten best soccer passing drills.", 1, "Passing Drills", "Soccer", "https://www.youtube.com/watch?v=Kb58F3r_TQM" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Defense", "Stop getting beaten in one-on-one situations.", 2, "Defensive Fundamentals", "Soccer", "https://www.youtube.com/watch?v=aadebgx5nz4" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Conditioning", "Eight exercises to improve speed, agility and power.", "Speed and Agility", "Soccer", "https://www.youtube.com/watch?v=cCZSTGeSuHM" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "Improve handles with advanced dribbling techniques.", "https://www.youtube.com/watch?v=UVAz2aASZx4" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "Master proper shooting technique and form.", "https://www.youtube.com/watch?v=hYaLYGmS61A" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "VideoUrl" },
                values: new object[] { "Learn proper defensive stance and movement.", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Passing", "Develop precision passing and court vision.", "Passing Accuracy", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Footwork", "Build explosive footwork and lateral agility.", 3, "Footwork Conditioning", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name", "VideoUrl" },
                values: new object[] { "Master offensive route running patterns.", "Passing Routes", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Defense", "Learn proper tackling form and safety.", "Tackling Technique", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Description", "Name", "VideoUrl" },
                values: new object[] { "Blocking", "Develop offensive line blocking fundamentals.", "Line Blocking", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "VideoUrl" },
                values: new object[] { "Ball Handling", "Prevent fumbles with strong ball security drills.", 1, "Ball Security", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name", "VideoUrl" },
                values: new object[] { "Build speed and endurance for game performance.", "Conditioning Sprints", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Perfect your stance and swing mechanics.", "Batting Stance", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Running", "Improve speed and technique between bases.", "Base Running", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Master ground ball and fly ball techniques.", 2, "Fielding Basics", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Pitching", "Learn proper pitching form and control.", 3, "Pitching Mechanics", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Throwing", "Develop arm strength and throwing precision.", 2, "Throwing Accuracy", "Baseball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Tee placement and batting technique.", 2, "Softball Hitting Drills", "Softball", "https://www.youtube.com/watch?v=ZxkrWk5j2NE" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Defense", "Learn proper fielding positions and angles.", 1, "Defensive Positioning", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Running", "Master safe and effective sliding into bases.", 2, "Sliding Technique", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Throwing", "Build arm strength with proper throwing form.", "Throwing Fundamentals", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Improve catching technique and pop time.", "Catching Skills", "Softball", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Dribbling", "Enhance touch and close ball control.", "Ball Control", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Passing", "Master short and long range passing.", 1, "Passing Accuracy", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Improve shot power and accuracy toward goal.", "Shooting Technique", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Defense", "Learn positioning and marking techniques.", 2, "Defensive Tactics", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Ball Control", "Develop soft first touch and ball reception.", 1, "First Touch", "Soccer", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Master puck control at speed.", 2, "Stick Handling", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Improve shot speed and placement.", "Shooting Accuracy", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Defense", "Learn proper body checking form.", 3, "Checking Technique", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Category", "Description", "DifficultyLevel", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Passing", "Develop precision tape-to-tape passing.", 1, "Passing Drills", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Category", "Description", "Name", "Sport", "VideoUrl" },
                values: new object[] { "Skating", "Build speed, balance, and edge work.", "Skating Agility", "Hockey", "https://www.youtube.com/watch?v=PLACEHOLDER" });
        }
    }
}
