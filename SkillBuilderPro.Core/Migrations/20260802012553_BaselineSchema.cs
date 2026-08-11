using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class BaselineSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Sport = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TargetArea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExperienceLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    JerseyNumber = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgressLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrillId = table.Column<int>(type: "int", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressLogs_Drills_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrillId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Drills_DrillId",
                        column: x => x.DrillId,
                        principalTable: "Drills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Drills",
                columns: new[] { "Id", "Category", "DateCreated", "Description", "Difficulty", "Duration", "Name", "Sport", "SubCategory", "VideoUrl" },
                values: new object[,]
                {
                    { 1, "Dribbling", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5-minute dribbling workout", 2, 0, "Ball Handling Drills", "Basketball", "", "" },
                    { 2, "Shooting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Find your perfect shooting form", 2, 0, "Shooting Form Basics", "Basketball", "", "" },
                    { 3, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Three defense drills", 2, 0, "Defensive Footwork", "Basketball", "", "" },
                    { 4, "Rebounding", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Basketball rebounding drills", 1, 0, "Rebounding Techniques", "Basketball", "", "" },
                    { 5, "Passing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Drills to become better at passing", 1, 0, "Passing Accuracy", "Basketball", "", "" },
                    { 6, "Offense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Transition offense drills", 3, 0, "Fast Break Drills", "Basketball", "", "" },
                    { 7, "Post", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Low post fundamentals", 2, 0, "Post Moves", "Basketball", "", "" },
                    { 8, "Shooting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Long range shooting drills", 3, 0, "Three Point Shooting", "Basketball", "", "" },
                    { 9, "Dribbling", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Advanced ball handling", 3, 0, "Crossover Dribble", "Basketball", "", "" },
                    { 10, "Passing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Passing technique drills", 1, 0, "Bounce Pass Drills", "Basketball", "", "" },
                    { 11, "Passing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Throwing mechanics", 2, 0, "Passing Technique", "Football", "", "" },
                    { 12, "Receiving", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "WR drills", 2, 0, "Catching Skills", "Football", "", "" },
                    { 13, "Receiving", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Creating separation", 2, 0, "Route Running", "Football", "", "" },
                    { 14, "Blocking", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Run and pass blocking", 2, 0, "Blocking Fundamentals", "Football", "", "" },
                    { 15, "Conditioning", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ladder drills", 3, 0, "Speed and Agility", "Football", "", "" },
                    { 16, "Footwork", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "QB footwork", 2, 0, "Footwork Drills", "Football", "", "" },
                    { 17, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pass rush drills", 3, 0, "Edge Rusher Technique", "Football", "", "" },
                    { 18, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Defensive back drills", 2, 0, "Coverage Drills", "Football", "", "" },
                    { 19, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Proper tackling form", 2, 0, "Tackling Drills", "Football", "", "" },
                    { 20, "Agility", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Agility cone drills", 1, 0, "Cone Drills", "Football", "", "" },
                    { 21, "Hitting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batting practice", 1, 0, "Hitting Drills", "Softball", "", "" },
                    { 22, "Pitching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Windmill pitch", 2, 0, "Pitching Mechanics", "Softball", "", "" },
                    { 23, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ground ball drills", 1, 0, "Infield Drills", "Softball", "", "" },
                    { 24, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fly ball drills", 2, 0, "Outfield Skills", "Softball", "", "" },
                    { 25, "Catching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Catching drills", 3, 0, "Catcher Fundamentals", "Softball", "", "" },
                    { 26, "Hitting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bunting drills", 2, 0, "Bunting Techniques", "Softball", "", "" },
                    { 27, "Running", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Running drills", 1, 0, "Base Running", "Softball", "", "" },
                    { 28, "Pitching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pitcher drills", 2, 0, "Drop Ball Drill", "Softball", "", "" },
                    { 29, "Throwing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Throwing accuracy", 2, 0, "Relay Throws", "Softball", "", "" },
                    { 30, "Running", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Base sliding", 1, 0, "Sliding Drills", "Softball", "", "" },
                    { 31, "Hitting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Batting practice", 1, 0, "Hitting Drills", "Baseball", "", "" },
                    { 32, "Pitching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Youth pitching drills", 2, 0, "Pitching Drills", "Baseball", "", "" },
                    { 33, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ground ball drills", 1, 0, "Infield Drills", "Baseball", "", "" },
                    { 34, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fly ball drills", 2, 0, "Outfield Drills", "Baseball", "", "" },
                    { 35, "Catching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Catcher drills", 3, 0, "Catcher Training", "Baseball", "", "" },
                    { 36, "Running", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Running drills", 1, 0, "Base Running", "Baseball", "", "" },
                    { 37, "Pitching", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Breaking ball drills", 3, 0, "Curveball Practice", "Baseball", "", "" },
                    { 38, "Throwing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Relay throw drills", 2, 0, "Cut-off Throws", "Baseball", "", "" },
                    { 39, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Infield coordination", 2, 0, "Double Play Drills", "Baseball", "", "" },
                    { 40, "Running", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Base sliding", 1, 0, "Slide Drills", "Baseball", "", "" },
                    { 41, "Skating", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Skating drills", 2, 0, "Edge-Work Skating", "Hockey", "", "" },
                    { 42, "Stickhandling", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dribbling drills", 2, 0, "Stickhandling Routine", "Hockey", "", "" },
                    { 43, "Shooting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shot accuracy", 2, 0, "Shooting Drills", "Hockey", "", "" },
                    { 44, "Passing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Passing drills", 1, 0, "Passing Technique", "Hockey", "", "" },
                    { 45, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Defense drills", 2, 0, "Defensive Positioning", "Hockey", "", "" },
                    { 46, "Skating", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Skating technique", 2, 0, "Crossover Drill", "Hockey", "", "" },
                    { 47, "Shooting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shooting on the move", 3, 0, "One-Timer Practice", "Hockey", "", "" },
                    { 48, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Body checking drills", 2, 0, "Checking Drills", "Hockey", "", "" },
                    { 49, "Goaltending", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Goaltending drills", 3, 0, "Goalie Drills", "Hockey", "", "" },
                    { 50, "Conditioning", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Endurance skating", 2, 0, "Conditioning Skate", "Hockey", "", "" },
                    { 51, "Dribbling", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ball control drills", 1, 0, "Dribbling Drills", "Soccer", "", "" },
                    { 52, "Shooting", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shooting drills", 2, 0, "Finishing Exercises", "Soccer", "", "" },
                    { 53, "Passing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Passing technique", 1, 0, "Passing Drills", "Soccer", "", "" },
                    { 54, "Defense", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Defense drills", 2, 0, "Defensive Fundamentals", "Soccer", "", "" },
                    { 55, "Conditioning", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Speed drills", 3, 0, "Speed and Agility", "Soccer", "", "" },
                    { 56, "Crossing", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wing play drills", 2, 0, "Crossing Drills", "Soccer", "", "" },
                    { 57, "Set Pieces", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Free kick drills", 3, 0, "Free Kick Practice", "Soccer", "", "" },
                    { 58, "Heading", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Heading technique", 2, 0, "Heading Drills", "Soccer", "", "" },
                    { 59, "Control", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ball control drills", 1, 0, "First Touch Drills", "Soccer", "", "" },
                    { 60, "Tactics", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Formation drills", 2, 0, "Tactical Drills", "Soccer", "", "" },
                    { 61, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced double plays drill for baseball players.", 2, 15, "Top 10 Infield Drills for Baseball Players", "Baseball", "Double Plays", "https://www.youtube.com/embed/bda0sQy7OIc" },
                    { 62, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced double plays drill for baseball players.", 3, 15, "12 Infield Drills Every Baseball Player Should Do", "Baseball", "Double Plays", "https://www.youtube.com/embed/A_FsvqFgu4c" },
                    { 63, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced double plays drill for baseball players.", 4, 15, "How to Improve Baseball Infield Footwork — 3 Drills", "Baseball", "Double Plays", "https://www.youtube.com/embed/cCshywB-m4Q" },
                    { 64, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced double plays drill for baseball players.", 5, 15, "Double Play Drills with Bill Ripken", "Baseball", "Double Plays", "https://www.youtube.com/embed/4aWrAxBqd7g" },
                    { 65, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for baseball players.", 2, 10, "Top 10 Infield Drills for Baseball Players", "Baseball", "Fly Balls", "https://www.youtube.com/embed/bda0sQy7OIc" },
                    { 66, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for baseball players.", 3, 10, "12 Infield Drills Every Baseball Player Should Do", "Baseball", "Fly Balls", "https://www.youtube.com/embed/A_FsvqFgu4c" },
                    { 67, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for baseball players.", 4, 10, "How to Improve Baseball Infield Footwork — 3 Drills", "Baseball", "Fly Balls", "https://www.youtube.com/embed/cCshywB-m4Q" },
                    { 68, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for baseball players.", 5, 10, "Double Play Drills with Bill Ripken", "Baseball", "Fly Balls", "https://www.youtube.com/embed/4aWrAxBqd7g" },
                    { 69, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for baseball players.", 2, 10, "Top 10 Infield Drills for Baseball Players", "Baseball", "Footwork", "https://www.youtube.com/embed/bda0sQy7OIc" },
                    { 70, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for baseball players.", 3, 10, "12 Infield Drills Every Baseball Player Should Do", "Baseball", "Footwork", "https://www.youtube.com/embed/A_FsvqFgu4c" },
                    { 71, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for baseball players.", 4, 10, "How to Improve Baseball Infield Footwork — 3 Drills", "Baseball", "Footwork", "https://www.youtube.com/embed/cCshywB-m4Q" },
                    { 72, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for baseball players.", 5, 10, "Double Play Drills with Bill Ripken", "Baseball", "Footwork", "https://www.youtube.com/embed/4aWrAxBqd7g" },
                    { 73, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for baseball players.", 2, 12, "Top 10 Infield Drills for Baseball Players", "Baseball", "Ground Balls", "https://www.youtube.com/embed/bda0sQy7OIc" },
                    { 74, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for baseball players.", 3, 12, "12 Infield Drills Every Baseball Player Should Do", "Baseball", "Ground Balls", "https://www.youtube.com/embed/A_FsvqFgu4c" },
                    { 75, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for baseball players.", 4, 12, "How to Improve Baseball Infield Footwork — 3 Drills", "Baseball", "Ground Balls", "https://www.youtube.com/embed/cCshywB-m4Q" },
                    { 76, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for baseball players.", 5, 12, "Double Play Drills with Bill Ripken", "Baseball", "Ground Balls", "https://www.youtube.com/embed/4aWrAxBqd7g" },
                    { 77, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for baseball players.", 2, 12, "Top 10 Infield Drills for Baseball Players", "Baseball", "Throwing", "https://www.youtube.com/embed/bda0sQy7OIc" },
                    { 78, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for baseball players.", 3, 12, "12 Infield Drills Every Baseball Player Should Do", "Baseball", "Throwing", "https://www.youtube.com/embed/A_FsvqFgu4c" },
                    { 79, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for baseball players.", 4, 12, "How to Improve Baseball Infield Footwork — 3 Drills", "Baseball", "Throwing", "https://www.youtube.com/embed/cCshywB-m4Q" },
                    { 80, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for baseball players.", 5, 12, "Double Play Drills with Bill Ripken", "Baseball", "Throwing", "https://www.youtube.com/embed/4aWrAxBqd7g" },
                    { 81, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for baseball players.", 2, 12, "4 Baseball Hitting Drills for Proper Bat Path", "Baseball", "Bat Path — Contact", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 82, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for baseball players.", 3, 12, "How to Hit a Baseball — Back to Basics", "Baseball", "Bat Path — Contact", "https://www.youtube.com/embed/YY9tErIBVQw" },
                    { 83, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for baseball players.", 4, 12, "How to Fix the 2 Biggest Baseball Hitting Flaws — 4 Drills", "Baseball", "Bat Path — Contact", "https://www.youtube.com/embed/5UWU9DVoHXc" },
                    { 84, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for baseball players.", 5, 12, "8 At-Home Baseball Hitting and Fielding Drills", "Baseball", "Bat Path — Contact", "https://www.youtube.com/embed/xooGNesMKpw" },
                    { 85, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for baseball players.", 2, 12, "4 Baseball Hitting Drills for Proper Bat Path", "Baseball", "Load", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 86, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for baseball players.", 3, 12, "How to Hit a Baseball — Back to Basics", "Baseball", "Load", "https://www.youtube.com/embed/YY9tErIBVQw" },
                    { 87, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for baseball players.", 4, 12, "How to Fix the 2 Biggest Baseball Hitting Flaws — 4 Drills", "Baseball", "Load", "https://www.youtube.com/embed/5UWU9DVoHXc" },
                    { 88, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for baseball players.", 5, 12, "8 At-Home Baseball Hitting and Fielding Drills", "Baseball", "Load", "https://www.youtube.com/embed/xooGNesMKpw" },
                    { 89, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for baseball players.", 2, 12, "4 Baseball Hitting Drills for Proper Bat Path", "Baseball", "Pitch Recognition", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 90, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for baseball players.", 3, 12, "How to Hit a Baseball — Back to Basics", "Baseball", "Pitch Recognition", "https://www.youtube.com/embed/YY9tErIBVQw" },
                    { 91, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for baseball players.", 4, 12, "How to Fix the 2 Biggest Baseball Hitting Flaws — 4 Drills", "Baseball", "Pitch Recognition", "https://www.youtube.com/embed/5UWU9DVoHXc" },
                    { 92, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for baseball players.", 5, 12, "8 At-Home Baseball Hitting and Fielding Drills", "Baseball", "Pitch Recognition", "https://www.youtube.com/embed/xooGNesMKpw" },
                    { 93, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for baseball players.", 2, 12, "4 Baseball Hitting Drills for Proper Bat Path", "Baseball", "Rotation — Power", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 94, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for baseball players.", 3, 12, "How to Hit a Baseball — Back to Basics", "Baseball", "Rotation — Power", "https://www.youtube.com/embed/YY9tErIBVQw" },
                    { 95, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for baseball players.", 4, 12, "How to Fix the 2 Biggest Baseball Hitting Flaws — 4 Drills", "Baseball", "Rotation — Power", "https://www.youtube.com/embed/5UWU9DVoHXc" },
                    { 96, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for baseball players.", 5, 12, "8 At-Home Baseball Hitting and Fielding Drills", "Baseball", "Rotation — Power", "https://www.youtube.com/embed/xooGNesMKpw" },
                    { 97, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for baseball players.", 2, 12, "4 Baseball Hitting Drills for Proper Bat Path", "Baseball", "Timing", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 98, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for baseball players.", 3, 12, "How to Hit a Baseball — Back to Basics", "Baseball", "Timing", "https://www.youtube.com/embed/YY9tErIBVQw" },
                    { 99, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for baseball players.", 4, 12, "How to Fix the 2 Biggest Baseball Hitting Flaws — 4 Drills", "Baseball", "Timing", "https://www.youtube.com/embed/5UWU9DVoHXc" },
                    { 100, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for baseball players.", 5, 12, "8 At-Home Baseball Hitting and Fielding Drills", "Baseball", "Timing", "https://www.youtube.com/embed/xooGNesMKpw" },
                    { 101, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for basketball players.", 2, 10, "5 Best Youth Basketball Defense Drills", "Basketball", "Footwork", "https://www.youtube.com/embed/XTIfQoI-kEI" },
                    { 102, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for basketball players.", 3, 10, "7 Drills to Teach Elite Basketball Footwork", "Basketball", "Footwork", "https://www.youtube.com/embed/nFNXFc0dvzI" },
                    { 103, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for basketball players.", 4, 10, "3 Shell Defense Variations for Lockdown Defense", "Basketball", "Footwork", "https://www.youtube.com/embed/Vds7fGlf4WU" },
                    { 104, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for basketball players.", 5, 10, "6 Footwork Progressions for Elite Defenders", "Basketball", "Footwork", "https://www.youtube.com/embed/s46bufWWJ6c" },
                    { 105, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced man-to-man drill for basketball players.", 2, 12, "5 Best Youth Basketball Defense Drills", "Basketball", "Man-to-Man", "https://www.youtube.com/embed/XTIfQoI-kEI" },
                    { 106, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced man-to-man drill for basketball players.", 3, 12, "7 Drills to Teach Elite Basketball Footwork", "Basketball", "Man-to-Man", "https://www.youtube.com/embed/nFNXFc0dvzI" },
                    { 107, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced man-to-man drill for basketball players.", 4, 12, "3 Shell Defense Variations for Lockdown Defense", "Basketball", "Man-to-Man", "https://www.youtube.com/embed/Vds7fGlf4WU" },
                    { 108, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced man-to-man drill for basketball players.", 5, 12, "6 Footwork Progressions for Elite Defenders", "Basketball", "Man-to-Man", "https://www.youtube.com/embed/s46bufWWJ6c" },
                    { 109, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for basketball players.", 2, 12, "5 Best Youth Basketball Defense Drills", "Basketball", "Positioning", "https://www.youtube.com/embed/XTIfQoI-kEI" },
                    { 110, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for basketball players.", 3, 12, "7 Drills to Teach Elite Basketball Footwork", "Basketball", "Positioning", "https://www.youtube.com/embed/nFNXFc0dvzI" },
                    { 111, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for basketball players.", 4, 12, "3 Shell Defense Variations for Lockdown Defense", "Basketball", "Positioning", "https://www.youtube.com/embed/Vds7fGlf4WU" },
                    { 112, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for basketball players.", 5, 12, "6 Footwork Progressions for Elite Defenders", "Basketball", "Positioning", "https://www.youtube.com/embed/s46bufWWJ6c" },
                    { 113, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 2, 12, "5 Best Youth Basketball Defense Drills", "Basketball", "Rebounding", "https://www.youtube.com/embed/XTIfQoI-kEI" },
                    { 114, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 3, 12, "7 Drills to Teach Elite Basketball Footwork", "Basketball", "Rebounding", "https://www.youtube.com/embed/nFNXFc0dvzI" },
                    { 115, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 4, 12, "3 Shell Defense Variations for Lockdown Defense", "Basketball", "Rebounding", "https://www.youtube.com/embed/Vds7fGlf4WU" },
                    { 116, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 5, 12, "6 Footwork Progressions for Elite Defenders", "Basketball", "Rebounding", "https://www.youtube.com/embed/s46bufWWJ6c" },
                    { 117, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced zone drill for basketball players.", 2, 12, "5 Best Youth Basketball Defense Drills", "Basketball", "Zone", "https://www.youtube.com/embed/XTIfQoI-kEI" },
                    { 118, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced zone drill for basketball players.", 3, 12, "7 Drills to Teach Elite Basketball Footwork", "Basketball", "Zone", "https://www.youtube.com/embed/nFNXFc0dvzI" },
                    { 119, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced zone drill for basketball players.", 4, 12, "3 Shell Defense Variations for Lockdown Defense", "Basketball", "Zone", "https://www.youtube.com/embed/Vds7fGlf4WU" },
                    { 120, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced zone drill for basketball players.", 5, 12, "6 Footwork Progressions for Elite Defenders", "Basketball", "Zone", "https://www.youtube.com/embed/s46bufWWJ6c" },
                    { 121, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for basketball players.", 2, 12, "5 Best Basketball Drills for Ages 10–12", "Basketball", "Dribbling", "https://www.youtube.com/embed/hpdu2NX_PQ0" },
                    { 122, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for basketball players.", 3, 12, "3 Best 3v3 Basketball Drills for Offense and Defense", "Basketball", "Dribbling", "https://www.youtube.com/embed/ZXbfHpmOUoU" },
                    { 123, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for basketball players.", 4, 12, "5 Best Drills for Elite Passing", "Basketball", "Dribbling", "https://www.youtube.com/embed/7rWBuiFlZ7k" },
                    { 124, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for basketball players.", 5, 12, "3 Finishing Drills to Train Game Situations", "Basketball", "Dribbling", "https://www.youtube.com/embed/tdAZo0CijX8" },
                    { 125, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced driving drill for basketball players.", 2, 12, "5 Best Basketball Drills for Ages 10–12", "Basketball", "Driving", "https://www.youtube.com/embed/hpdu2NX_PQ0" },
                    { 126, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced driving drill for basketball players.", 3, 12, "3 Best 3v3 Basketball Drills for Offense and Defense", "Basketball", "Driving", "https://www.youtube.com/embed/ZXbfHpmOUoU" },
                    { 127, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced driving drill for basketball players.", 4, 12, "5 Best Drills for Elite Passing", "Basketball", "Driving", "https://www.youtube.com/embed/7rWBuiFlZ7k" },
                    { 128, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced driving drill for basketball players.", 5, 12, "3 Finishing Drills to Train Game Situations", "Basketball", "Driving", "https://www.youtube.com/embed/tdAZo0CijX8" },
                    { 129, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for basketball players.", 2, 12, "5 Best Basketball Drills for Ages 10–12", "Basketball", "Passing", "https://www.youtube.com/embed/hpdu2NX_PQ0" },
                    { 130, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for basketball players.", 3, 12, "3 Best 3v3 Basketball Drills for Offense and Defense", "Basketball", "Passing", "https://www.youtube.com/embed/ZXbfHpmOUoU" },
                    { 131, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for basketball players.", 4, 12, "5 Best Drills for Elite Passing", "Basketball", "Passing", "https://www.youtube.com/embed/7rWBuiFlZ7k" },
                    { 132, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for basketball players.", 5, 12, "3 Finishing Drills to Train Game Situations", "Basketball", "Passing", "https://www.youtube.com/embed/tdAZo0CijX8" },
                    { 133, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 2, 12, "5 Best Basketball Drills for Ages 10–12", "Basketball", "Rebounding", "https://www.youtube.com/embed/hpdu2NX_PQ0" },
                    { 134, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 3, 12, "3 Best 3v3 Basketball Drills for Offense and Defense", "Basketball", "Rebounding", "https://www.youtube.com/embed/ZXbfHpmOUoU" },
                    { 135, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 4, 12, "5 Best Drills for Elite Passing", "Basketball", "Rebounding", "https://www.youtube.com/embed/7rWBuiFlZ7k" },
                    { 136, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rebounding drill for basketball players.", 5, 12, "3 Finishing Drills to Train Game Situations", "Basketball", "Rebounding", "https://www.youtube.com/embed/tdAZo0CijX8" },
                    { 137, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for basketball players.", 2, 12, "5 Best Basketball Drills for Ages 10–12", "Basketball", "Shooting", "https://www.youtube.com/embed/hpdu2NX_PQ0" },
                    { 138, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for basketball players.", 3, 12, "3 Best 3v3 Basketball Drills for Offense and Defense", "Basketball", "Shooting", "https://www.youtube.com/embed/ZXbfHpmOUoU" },
                    { 139, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for basketball players.", 4, 12, "5 Best Drills for Elite Passing", "Basketball", "Shooting", "https://www.youtube.com/embed/7rWBuiFlZ7k" },
                    { 140, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for basketball players.", 5, 12, "3 Finishing Drills to Train Game Situations", "Basketball", "Shooting", "https://www.youtube.com/embed/tdAZo0CijX8" },
                    { 141, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced coverage drill for football players.", 2, 12, "DB Run Support: 4 Drills That Fix Poor Angles", "Football", "Coverage", "https://www.youtube.com/embed/NMdRtzM3n4c" },
                    { 142, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced coverage drill for football players.", 3, 12, "2 Angle Tackle Drills for Explosiveness", "Football", "Coverage", "https://www.youtube.com/embed/wNytm-rLPO0" },
                    { 143, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced coverage drill for football players.", 4, 12, "2 Game-Changing Linebacker Drills", "Football", "Coverage", "https://www.youtube.com/embed/hnzKRY0jwvk" },
                    { 144, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced coverage drill for football players.", 5, 12, "Defensive Back Technique and Footwork Drills", "Football", "Coverage", "https://www.youtube.com/embed/SZq2slIu4Ys" },
                    { 145, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced film study drill for football players.", 2, 12, "DB Run Support: 4 Drills That Fix Poor Angles", "Football", "Film Study", "https://www.youtube.com/embed/NMdRtzM3n4c" },
                    { 146, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced film study drill for football players.", 3, 12, "2 Angle Tackle Drills for Explosiveness", "Football", "Film Study", "https://www.youtube.com/embed/wNytm-rLPO0" },
                    { 147, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced film study drill for football players.", 4, 12, "2 Game-Changing Linebacker Drills", "Football", "Film Study", "https://www.youtube.com/embed/hnzKRY0jwvk" },
                    { 148, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced film study drill for football players.", 5, 12, "Defensive Back Technique and Footwork Drills", "Football", "Film Study", "https://www.youtube.com/embed/SZq2slIu4Ys" },
                    { 149, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 2, 10, "DB Run Support: 4 Drills That Fix Poor Angles", "Football", "Footwork", "https://www.youtube.com/embed/NMdRtzM3n4c" },
                    { 150, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 3, 10, "2 Angle Tackle Drills for Explosiveness", "Football", "Footwork", "https://www.youtube.com/embed/wNytm-rLPO0" },
                    { 151, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 4, 10, "2 Game-Changing Linebacker Drills", "Football", "Footwork", "https://www.youtube.com/embed/hnzKRY0jwvk" },
                    { 152, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 5, 10, "Defensive Back Technique and Footwork Drills", "Football", "Footwork", "https://www.youtube.com/embed/SZq2slIu4Ys" },
                    { 153, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced gap integrity drill for football players.", 2, 12, "DB Run Support: 4 Drills That Fix Poor Angles", "Football", "Gap Integrity", "https://www.youtube.com/embed/NMdRtzM3n4c" },
                    { 154, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced gap integrity drill for football players.", 3, 12, "2 Angle Tackle Drills for Explosiveness", "Football", "Gap Integrity", "https://www.youtube.com/embed/wNytm-rLPO0" },
                    { 155, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced gap integrity drill for football players.", 4, 12, "2 Game-Changing Linebacker Drills", "Football", "Gap Integrity", "https://www.youtube.com/embed/hnzKRY0jwvk" },
                    { 156, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced gap integrity drill for football players.", 5, 12, "Defensive Back Technique and Footwork Drills", "Football", "Gap Integrity", "https://www.youtube.com/embed/SZq2slIu4Ys" },
                    { 157, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced tackling drill for football players.", 2, 12, "DB Run Support: 4 Drills That Fix Poor Angles", "Football", "Tackling", "https://www.youtube.com/embed/NMdRtzM3n4c" },
                    { 158, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced tackling drill for football players.", 3, 12, "2 Angle Tackle Drills for Explosiveness", "Football", "Tackling", "https://www.youtube.com/embed/wNytm-rLPO0" },
                    { 159, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced tackling drill for football players.", 4, 12, "2 Game-Changing Linebacker Drills", "Football", "Tackling", "https://www.youtube.com/embed/hnzKRY0jwvk" },
                    { 160, "Defense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced tackling drill for football players.", 5, 12, "Defensive Back Technique and Footwork Drills", "Football", "Tackling", "https://www.youtube.com/embed/SZq2slIu4Ys" },
                    { 161, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ball security drill for football players.", 2, 12, "Five Passing and Receiving Routes — QB/WR Drill", "Football", "Ball Security", "https://www.youtube.com/embed/DHVT0R1njzM" },
                    { 162, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ball security drill for football players.", 3, 12, "Christian McCaffrey Running Back Drills", "Football", "Ball Security", "https://www.youtube.com/embed/g9V6Rf6xPNg" },
                    { 163, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ball security drill for football players.", 4, 12, "Daily RB Drills: Ball Security, Blocking and Footwork", "Football", "Ball Security", "https://www.youtube.com/embed/wBdB9pFvQ4o" },
                    { 164, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ball security drill for football players.", 5, 12, "Six Everyday Wide Receiver Drills", "Football", "Ball Security", "https://www.youtube.com/embed/FirCA5QeiE0" },
                    { 165, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced decision making drill for football players.", 2, 12, "Five Passing and Receiving Routes — QB/WR Drill", "Football", "Decision Making", "https://www.youtube.com/embed/DHVT0R1njzM" },
                    { 166, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced decision making drill for football players.", 3, 12, "Christian McCaffrey Running Back Drills", "Football", "Decision Making", "https://www.youtube.com/embed/g9V6Rf6xPNg" },
                    { 167, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced decision making drill for football players.", 4, 12, "Daily RB Drills: Ball Security, Blocking and Footwork", "Football", "Decision Making", "https://www.youtube.com/embed/wBdB9pFvQ4o" },
                    { 168, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced decision making drill for football players.", 5, 12, "Six Everyday Wide Receiver Drills", "Football", "Decision Making", "https://www.youtube.com/embed/FirCA5QeiE0" },
                    { 169, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 2, 10, "Five Passing and Receiving Routes — QB/WR Drill", "Football", "Footwork", "https://www.youtube.com/embed/DHVT0R1njzM" },
                    { 170, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 3, 10, "Christian McCaffrey Running Back Drills", "Football", "Footwork", "https://www.youtube.com/embed/g9V6Rf6xPNg" },
                    { 171, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 4, 10, "Daily RB Drills: Ball Security, Blocking and Footwork", "Football", "Footwork", "https://www.youtube.com/embed/wBdB9pFvQ4o" },
                    { 172, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for football players.", 5, 10, "Six Everyday Wide Receiver Drills", "Football", "Footwork", "https://www.youtube.com/embed/FirCA5QeiE0" },
                    { 173, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for football players.", 2, 12, "Five Passing and Receiving Routes — QB/WR Drill", "Football", "Passing", "https://www.youtube.com/embed/DHVT0R1njzM" },
                    { 174, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for football players.", 3, 12, "Christian McCaffrey Running Back Drills", "Football", "Passing", "https://www.youtube.com/embed/g9V6Rf6xPNg" },
                    { 175, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for football players.", 4, 12, "Daily RB Drills: Ball Security, Blocking and Footwork", "Football", "Passing", "https://www.youtube.com/embed/wBdB9pFvQ4o" },
                    { 176, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for football players.", 5, 12, "Six Everyday Wide Receiver Drills", "Football", "Passing", "https://www.youtube.com/embed/FirCA5QeiE0" },
                    { 177, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced route running drill for football players.", 2, 12, "Five Passing and Receiving Routes — QB/WR Drill", "Football", "Route Running", "https://www.youtube.com/embed/DHVT0R1njzM" },
                    { 178, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced route running drill for football players.", 3, 12, "Christian McCaffrey Running Back Drills", "Football", "Route Running", "https://www.youtube.com/embed/g9V6Rf6xPNg" },
                    { 179, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced route running drill for football players.", 4, 12, "Daily RB Drills: Ball Security, Blocking and Footwork", "Football", "Route Running", "https://www.youtube.com/embed/wBdB9pFvQ4o" },
                    { 180, "Offense", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced route running drill for football players.", 5, 12, "Six Everyday Wide Receiver Drills", "Football", "Route Running", "https://www.youtube.com/embed/FirCA5QeiE0" },
                    { 181, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced accuracy drill for hockey players.", 2, 12, "Elite Hockey Snap Shot Tutorial", "Hockey", "Accuracy", "https://www.youtube.com/embed/OZnHLgWe964" },
                    { 182, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced accuracy drill for hockey players.", 3, 12, "Improve Your Wrist and Snap Shot", "Hockey", "Accuracy", "https://www.youtube.com/embed/0Mht5TbYNwA" },
                    { 183, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced accuracy drill for hockey players.", 4, 12, "Learn to Shoot a Wrist Shot — iTrain Hockey", "Hockey", "Accuracy", "https://www.youtube.com/embed/HQY14pYYWuQ" },
                    { 184, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced accuracy drill for hockey players.", 5, 12, "How to Slap Shot Like an NHL Player", "Hockey", "Accuracy", "https://www.youtube.com/embed/iuaK6HU7RzI" },
                    { 185, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced release drill for hockey players.", 2, 12, "Elite Hockey Snap Shot Tutorial", "Hockey", "Release", "https://www.youtube.com/embed/OZnHLgWe964" },
                    { 186, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced release drill for hockey players.", 3, 12, "Improve Your Wrist and Snap Shot", "Hockey", "Release", "https://www.youtube.com/embed/0Mht5TbYNwA" },
                    { 187, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced release drill for hockey players.", 4, 12, "Learn to Shoot a Wrist Shot — iTrain Hockey", "Hockey", "Release", "https://www.youtube.com/embed/HQY14pYYWuQ" },
                    { 188, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced release drill for hockey players.", 5, 12, "How to Slap Shot Like an NHL Player", "Hockey", "Release", "https://www.youtube.com/embed/iuaK6HU7RzI" },
                    { 189, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced slap shot drill for hockey players.", 2, 12, "Elite Hockey Snap Shot Tutorial", "Hockey", "Slap Shot", "https://www.youtube.com/embed/OZnHLgWe964" },
                    { 190, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced slap shot drill for hockey players.", 3, 12, "Improve Your Wrist and Snap Shot", "Hockey", "Slap Shot", "https://www.youtube.com/embed/0Mht5TbYNwA" },
                    { 191, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced slap shot drill for hockey players.", 4, 12, "Learn to Shoot a Wrist Shot — iTrain Hockey", "Hockey", "Slap Shot", "https://www.youtube.com/embed/HQY14pYYWuQ" },
                    { 192, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced slap shot drill for hockey players.", 5, 12, "How to Slap Shot Like an NHL Player", "Hockey", "Slap Shot", "https://www.youtube.com/embed/iuaK6HU7RzI" },
                    { 193, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced snap shot drill for hockey players.", 2, 12, "Elite Hockey Snap Shot Tutorial", "Hockey", "Snap Shot", "https://www.youtube.com/embed/OZnHLgWe964" },
                    { 194, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced snap shot drill for hockey players.", 3, 12, "Improve Your Wrist and Snap Shot", "Hockey", "Snap Shot", "https://www.youtube.com/embed/0Mht5TbYNwA" },
                    { 195, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced snap shot drill for hockey players.", 4, 12, "Learn to Shoot a Wrist Shot — iTrain Hockey", "Hockey", "Snap Shot", "https://www.youtube.com/embed/HQY14pYYWuQ" },
                    { 196, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced snap shot drill for hockey players.", 5, 12, "How to Slap Shot Like an NHL Player", "Hockey", "Snap Shot", "https://www.youtube.com/embed/iuaK6HU7RzI" },
                    { 197, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced wrist shot drill for hockey players.", 2, 12, "Elite Hockey Snap Shot Tutorial", "Hockey", "Wrist Shot", "https://www.youtube.com/embed/OZnHLgWe964" },
                    { 198, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced wrist shot drill for hockey players.", 3, 12, "Improve Your Wrist and Snap Shot", "Hockey", "Wrist Shot", "https://www.youtube.com/embed/0Mht5TbYNwA" },
                    { 199, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced wrist shot drill for hockey players.", 4, 12, "Learn to Shoot a Wrist Shot — iTrain Hockey", "Hockey", "Wrist Shot", "https://www.youtube.com/embed/HQY14pYYWuQ" },
                    { 200, "Shooting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced wrist shot drill for hockey players.", 5, 12, "How to Slap Shot Like an NHL Player", "Hockey", "Wrist Shot", "https://www.youtube.com/embed/iuaK6HU7RzI" },
                    { 201, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced agility drill for hockey players.", 2, 12, "Understanding Edges — Skating Fundamentals", "Hockey", "Agility", "https://www.youtube.com/embed/q8-aoYR-bYo" },
                    { 202, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced agility drill for hockey players.", 3, 12, "Insane Edge Work Drills", "Hockey", "Agility", "https://www.youtube.com/embed/k8KcoVsfyHA" },
                    { 203, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced agility drill for hockey players.", 4, 12, "Half Pivot Hockey Skating Drill", "Hockey", "Agility", "https://www.youtube.com/embed/jevSo0oTw5k" },
                    { 204, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced agility drill for hockey players.", 5, 12, "Inside Edges — Power Skating Progression", "Hockey", "Agility", "https://www.youtube.com/embed/7dA0S23DR50" },
                    { 205, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced conditioning drill for hockey players.", 2, 12, "Understanding Edges — Skating Fundamentals", "Hockey", "Conditioning", "https://www.youtube.com/embed/q8-aoYR-bYo" },
                    { 206, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced conditioning drill for hockey players.", 3, 12, "Insane Edge Work Drills", "Hockey", "Conditioning", "https://www.youtube.com/embed/k8KcoVsfyHA" },
                    { 207, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced conditioning drill for hockey players.", 4, 12, "Half Pivot Hockey Skating Drill", "Hockey", "Conditioning", "https://www.youtube.com/embed/jevSo0oTw5k" },
                    { 208, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced conditioning drill for hockey players.", 5, 12, "Inside Edges — Power Skating Progression", "Hockey", "Conditioning", "https://www.youtube.com/embed/7dA0S23DR50" },
                    { 209, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced edges drill for hockey players.", 2, 12, "Understanding Edges — Skating Fundamentals", "Hockey", "Edges", "https://www.youtube.com/embed/q8-aoYR-bYo" },
                    { 210, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced edges drill for hockey players.", 3, 12, "Insane Edge Work Drills", "Hockey", "Edges", "https://www.youtube.com/embed/k8KcoVsfyHA" },
                    { 211, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced edges drill for hockey players.", 4, 12, "Half Pivot Hockey Skating Drill", "Hockey", "Edges", "https://www.youtube.com/embed/jevSo0oTw5k" },
                    { 212, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced edges drill for hockey players.", 5, 12, "Inside Edges — Power Skating Progression", "Hockey", "Edges", "https://www.youtube.com/embed/7dA0S23DR50" },
                    { 213, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced speed drill for hockey players.", 2, 12, "Understanding Edges — Skating Fundamentals", "Hockey", "Speed", "https://www.youtube.com/embed/q8-aoYR-bYo" },
                    { 214, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced speed drill for hockey players.", 3, 12, "Insane Edge Work Drills", "Hockey", "Speed", "https://www.youtube.com/embed/k8KcoVsfyHA" },
                    { 215, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced speed drill for hockey players.", 4, 12, "Half Pivot Hockey Skating Drill", "Hockey", "Speed", "https://www.youtube.com/embed/jevSo0oTw5k" },
                    { 216, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced speed drill for hockey players.", 5, 12, "Inside Edges — Power Skating Progression", "Hockey", "Speed", "https://www.youtube.com/embed/7dA0S23DR50" },
                    { 217, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced transitions drill for hockey players.", 2, 12, "Understanding Edges — Skating Fundamentals", "Hockey", "Transitions", "https://www.youtube.com/embed/q8-aoYR-bYo" },
                    { 218, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced transitions drill for hockey players.", 3, 12, "Insane Edge Work Drills", "Hockey", "Transitions", "https://www.youtube.com/embed/k8KcoVsfyHA" },
                    { 219, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced transitions drill for hockey players.", 4, 12, "Half Pivot Hockey Skating Drill", "Hockey", "Transitions", "https://www.youtube.com/embed/jevSo0oTw5k" },
                    { 220, "Skating", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced transitions drill for hockey players.", 5, 12, "Inside Edges — Power Skating Progression", "Hockey", "Transitions", "https://www.youtube.com/embed/7dA0S23DR50" },
                    { 221, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for soccer players.", 2, 12, "10 Clinical Soccer Finishing Drills", "Soccer", "Dribbling", "https://www.youtube.com/embed/0u8kPwXXsLA" },
                    { 222, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for soccer players.", 3, 12, "10 Exercises to Master Your First Touch", "Soccer", "Dribbling", "https://www.youtube.com/embed/ud84rp3Vphs" },
                    { 223, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for soccer players.", 4, 12, "High-Tempo Crossing and Finishing Drills", "Soccer", "Dribbling", "https://www.youtube.com/embed/Dbsul_7cl0A" },
                    { 224, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced dribbling drill for soccer players.", 5, 12, "Full First-Touch and Finishing Training Session", "Soccer", "Dribbling", "https://www.youtube.com/embed/RfkgDDN8CK0" },
                    { 225, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced first touch drill for soccer players.", 2, 12, "10 Clinical Soccer Finishing Drills", "Soccer", "First Touch", "https://www.youtube.com/embed/0u8kPwXXsLA" },
                    { 226, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced first touch drill for soccer players.", 3, 12, "10 Exercises to Master Your First Touch", "Soccer", "First Touch", "https://www.youtube.com/embed/ud84rp3Vphs" },
                    { 227, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced first touch drill for soccer players.", 4, 12, "High-Tempo Crossing and Finishing Drills", "Soccer", "First Touch", "https://www.youtube.com/embed/Dbsul_7cl0A" },
                    { 228, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced first touch drill for soccer players.", 5, 12, "Full First-Touch and Finishing Training Session", "Soccer", "First Touch", "https://www.youtube.com/embed/RfkgDDN8CK0" },
                    { 229, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for soccer players.", 2, 12, "10 Clinical Soccer Finishing Drills", "Soccer", "Passing", "https://www.youtube.com/embed/0u8kPwXXsLA" },
                    { 230, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for soccer players.", 3, 12, "10 Exercises to Master Your First Touch", "Soccer", "Passing", "https://www.youtube.com/embed/ud84rp3Vphs" },
                    { 231, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for soccer players.", 4, 12, "High-Tempo Crossing and Finishing Drills", "Soccer", "Passing", "https://www.youtube.com/embed/Dbsul_7cl0A" },
                    { 232, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced passing drill for soccer players.", 5, 12, "Full First-Touch and Finishing Training Session", "Soccer", "Passing", "https://www.youtube.com/embed/RfkgDDN8CK0" },
                    { 233, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 2, 12, "10 Clinical Soccer Finishing Drills", "Soccer", "Positioning", "https://www.youtube.com/embed/0u8kPwXXsLA" },
                    { 234, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 3, 12, "10 Exercises to Master Your First Touch", "Soccer", "Positioning", "https://www.youtube.com/embed/ud84rp3Vphs" },
                    { 235, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 4, 12, "High-Tempo Crossing and Finishing Drills", "Soccer", "Positioning", "https://www.youtube.com/embed/Dbsul_7cl0A" },
                    { 236, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 5, 12, "Full First-Touch and Finishing Training Session", "Soccer", "Positioning", "https://www.youtube.com/embed/RfkgDDN8CK0" },
                    { 237, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for soccer players.", 2, 12, "10 Clinical Soccer Finishing Drills", "Soccer", "Shooting", "https://www.youtube.com/embed/0u8kPwXXsLA" },
                    { 238, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for soccer players.", 3, 12, "10 Exercises to Master Your First Touch", "Soccer", "Shooting", "https://www.youtube.com/embed/ud84rp3Vphs" },
                    { 239, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for soccer players.", 4, 12, "High-Tempo Crossing and Finishing Drills", "Soccer", "Shooting", "https://www.youtube.com/embed/Dbsul_7cl0A" },
                    { 240, "Attacking", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced shooting drill for soccer players.", 5, 12, "Full First-Touch and Finishing Training Session", "Soccer", "Shooting", "https://www.youtube.com/embed/RfkgDDN8CK0" },
                    { 241, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced marking drill for soccer players.", 2, 12, "How to Defend in Soccer — 3 Drills", "Soccer", "Marking", "https://www.youtube.com/embed/LR9ifmPXGhI" },
                    { 242, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced marking drill for soccer players.", 3, 12, "Dynamic Defending: Marking and Positioning", "Soccer", "Marking", "https://www.youtube.com/embed/4fqsA6IDpjw" },
                    { 243, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced marking drill for soccer players.", 4, 12, "Pressing, Covering and Delaying", "Soccer", "Marking", "https://www.youtube.com/embed/oZ6RUC3D1kU" },
                    { 244, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced marking drill for soccer players.", 5, 12, "How to Win More Tackles in Soccer", "Soccer", "Marking", "https://www.youtube.com/embed/D-WxjGRjTIU" },
                    { 245, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 2, 12, "How to Defend in Soccer — 3 Drills", "Soccer", "Positioning", "https://www.youtube.com/embed/LR9ifmPXGhI" },
                    { 246, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 3, 12, "Dynamic Defending: Marking and Positioning", "Soccer", "Positioning", "https://www.youtube.com/embed/4fqsA6IDpjw" },
                    { 247, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 4, 12, "Pressing, Covering and Delaying", "Soccer", "Positioning", "https://www.youtube.com/embed/oZ6RUC3D1kU" },
                    { 248, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced positioning drill for soccer players.", 5, 12, "How to Win More Tackles in Soccer", "Soccer", "Positioning", "https://www.youtube.com/embed/D-WxjGRjTIU" },
                    { 249, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pressing drill for soccer players.", 2, 12, "How to Defend in Soccer — 3 Drills", "Soccer", "Pressing", "https://www.youtube.com/embed/LR9ifmPXGhI" },
                    { 250, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pressing drill for soccer players.", 3, 12, "Dynamic Defending: Marking and Positioning", "Soccer", "Pressing", "https://www.youtube.com/embed/4fqsA6IDpjw" },
                    { 251, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pressing drill for soccer players.", 4, 12, "Pressing, Covering and Delaying", "Soccer", "Pressing", "https://www.youtube.com/embed/oZ6RUC3D1kU" },
                    { 252, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pressing drill for soccer players.", 5, 12, "How to Win More Tackles in Soccer", "Soccer", "Pressing", "https://www.youtube.com/embed/D-WxjGRjTIU" },
                    { 253, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced recovery drill for soccer players.", 2, 12, "How to Defend in Soccer — 3 Drills", "Soccer", "Recovery", "https://www.youtube.com/embed/LR9ifmPXGhI" },
                    { 254, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced recovery drill for soccer players.", 3, 12, "Dynamic Defending: Marking and Positioning", "Soccer", "Recovery", "https://www.youtube.com/embed/4fqsA6IDpjw" },
                    { 255, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced recovery drill for soccer players.", 4, 12, "Pressing, Covering and Delaying", "Soccer", "Recovery", "https://www.youtube.com/embed/oZ6RUC3D1kU" },
                    { 256, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced recovery drill for soccer players.", 5, 12, "How to Win More Tackles in Soccer", "Soccer", "Recovery", "https://www.youtube.com/embed/D-WxjGRjTIU" },
                    { 257, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding/tackling drill for soccer players.", 2, 12, "How to Defend in Soccer — 3 Drills", "Soccer", "Sliding/Tackling", "https://www.youtube.com/embed/LR9ifmPXGhI" },
                    { 258, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding/tackling drill for soccer players.", 3, 12, "Dynamic Defending: Marking and Positioning", "Soccer", "Sliding/Tackling", "https://www.youtube.com/embed/4fqsA6IDpjw" },
                    { 259, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding/tackling drill for soccer players.", 4, 12, "Pressing, Covering and Delaying", "Soccer", "Sliding/Tackling", "https://www.youtube.com/embed/oZ6RUC3D1kU" },
                    { 260, "Defending", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding/tackling drill for soccer players.", 5, 12, "How to Win More Tackles in Soccer", "Soccer", "Sliding/Tackling", "https://www.youtube.com/embed/D-WxjGRjTIU" },
                    { 261, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for softball players.", 2, 10, "Side-to-Side Shuffle and Throw Infield Drill", "Softball", "Fly Balls", "https://www.youtube.com/embed/rFEgj683qh0" },
                    { 262, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for softball players.", 3, 10, "Two Infield Drills for Quick, Efficient Movement", "Softball", "Fly Balls", "https://www.youtube.com/embed/w7-41ueexqo" },
                    { 263, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for softball players.", 4, 10, "Two Outfield Drills for Explosive Movement", "Softball", "Fly Balls", "https://www.youtube.com/embed/wYmTKx-8Sdk" },
                    { 264, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced fly balls drill for softball players.", 5, 10, "Improve Catching with These Infield Drills", "Softball", "Fly Balls", "https://www.youtube.com/embed/zBfiZZ9G9zg" },
                    { 265, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for softball players.", 2, 10, "Side-to-Side Shuffle and Throw Infield Drill", "Softball", "Footwork", "https://www.youtube.com/embed/rFEgj683qh0" },
                    { 266, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for softball players.", 3, 10, "Two Infield Drills for Quick, Efficient Movement", "Softball", "Footwork", "https://www.youtube.com/embed/w7-41ueexqo" },
                    { 267, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for softball players.", 4, 10, "Two Outfield Drills for Explosive Movement", "Softball", "Footwork", "https://www.youtube.com/embed/wYmTKx-8Sdk" },
                    { 268, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced footwork drill for softball players.", 5, 10, "Improve Catching with These Infield Drills", "Softball", "Footwork", "https://www.youtube.com/embed/zBfiZZ9G9zg" },
                    { 269, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for softball players.", 2, 12, "Side-to-Side Shuffle and Throw Infield Drill", "Softball", "Ground Balls", "https://www.youtube.com/embed/rFEgj683qh0" },
                    { 270, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for softball players.", 3, 12, "Two Infield Drills for Quick, Efficient Movement", "Softball", "Ground Balls", "https://www.youtube.com/embed/w7-41ueexqo" },
                    { 271, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for softball players.", 4, 12, "Two Outfield Drills for Explosive Movement", "Softball", "Ground Balls", "https://www.youtube.com/embed/wYmTKx-8Sdk" },
                    { 272, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced ground balls drill for softball players.", 5, 12, "Improve Catching with These Infield Drills", "Softball", "Ground Balls", "https://www.youtube.com/embed/zBfiZZ9G9zg" },
                    { 273, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding drill for softball players.", 2, 12, "Side-to-Side Shuffle and Throw Infield Drill", "Softball", "Sliding", "https://www.youtube.com/embed/rFEgj683qh0" },
                    { 274, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding drill for softball players.", 3, 12, "Two Infield Drills for Quick, Efficient Movement", "Softball", "Sliding", "https://www.youtube.com/embed/w7-41ueexqo" },
                    { 275, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding drill for softball players.", 4, 12, "Two Outfield Drills for Explosive Movement", "Softball", "Sliding", "https://www.youtube.com/embed/wYmTKx-8Sdk" },
                    { 276, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced sliding drill for softball players.", 5, 12, "Improve Catching with These Infield Drills", "Softball", "Sliding", "https://www.youtube.com/embed/zBfiZZ9G9zg" },
                    { 277, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for softball players.", 2, 12, "Side-to-Side Shuffle and Throw Infield Drill", "Softball", "Throwing", "https://www.youtube.com/embed/rFEgj683qh0" },
                    { 278, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for softball players.", 3, 12, "Two Infield Drills for Quick, Efficient Movement", "Softball", "Throwing", "https://www.youtube.com/embed/w7-41ueexqo" },
                    { 279, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for softball players.", 4, 12, "Two Outfield Drills for Explosive Movement", "Softball", "Throwing", "https://www.youtube.com/embed/wYmTKx-8Sdk" },
                    { 280, "Fielding", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced throwing drill for softball players.", 5, 12, "Improve Catching with These Infield Drills", "Softball", "Throwing", "https://www.youtube.com/embed/zBfiZZ9G9zg" },
                    { 281, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for softball players.", 2, 12, "4 Baseball/Softball Hitting Drills for Proper Bat Path", "Softball", "Bat Path — Contact", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 282, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for softball players.", 3, 12, "Optimal Posture for a Hitter — The Hitting Vault", "Softball", "Bat Path — Contact", "https://www.youtube.com/embed/U72mLOkvqrk" },
                    { 283, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for softball players.", 4, 12, "Top-Hand Drill for an Optimal Bat Path", "Softball", "Bat Path — Contact", "https://www.youtube.com/embed/W2-kOnlmbY0" },
                    { 284, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced bat path — contact drill for softball players.", 5, 12, "Excellent Hand-Path Drill for an Elite Compact Swing", "Softball", "Bat Path — Contact", "https://www.youtube.com/embed/KhyK-laglXc" },
                    { 285, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for softball players.", 2, 12, "4 Baseball/Softball Hitting Drills for Proper Bat Path", "Softball", "Load", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 286, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for softball players.", 3, 12, "Optimal Posture for a Hitter — The Hitting Vault", "Softball", "Load", "https://www.youtube.com/embed/U72mLOkvqrk" },
                    { 287, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for softball players.", 4, 12, "Top-Hand Drill for an Optimal Bat Path", "Softball", "Load", "https://www.youtube.com/embed/W2-kOnlmbY0" },
                    { 288, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced load drill for softball players.", 5, 12, "Excellent Hand-Path Drill for an Elite Compact Swing", "Softball", "Load", "https://www.youtube.com/embed/KhyK-laglXc" },
                    { 289, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for softball players.", 2, 12, "4 Baseball/Softball Hitting Drills for Proper Bat Path", "Softball", "Pitch Recognition", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 290, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for softball players.", 3, 12, "Optimal Posture for a Hitter — The Hitting Vault", "Softball", "Pitch Recognition", "https://www.youtube.com/embed/U72mLOkvqrk" },
                    { 291, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for softball players.", 4, 12, "Top-Hand Drill for an Optimal Bat Path", "Softball", "Pitch Recognition", "https://www.youtube.com/embed/W2-kOnlmbY0" },
                    { 292, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced pitch recognition drill for softball players.", 5, 12, "Excellent Hand-Path Drill for an Elite Compact Swing", "Softball", "Pitch Recognition", "https://www.youtube.com/embed/KhyK-laglXc" },
                    { 293, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for softball players.", 2, 12, "4 Baseball/Softball Hitting Drills for Proper Bat Path", "Softball", "Rotation — Power", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 294, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for softball players.", 3, 12, "Optimal Posture for a Hitter — The Hitting Vault", "Softball", "Rotation — Power", "https://www.youtube.com/embed/U72mLOkvqrk" },
                    { 295, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for softball players.", 4, 12, "Top-Hand Drill for an Optimal Bat Path", "Softball", "Rotation — Power", "https://www.youtube.com/embed/W2-kOnlmbY0" },
                    { 296, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced rotation — power drill for softball players.", 5, 12, "Excellent Hand-Path Drill for an Elite Compact Swing", "Softball", "Rotation — Power", "https://www.youtube.com/embed/KhyK-laglXc" },
                    { 297, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for softball players.", 2, 12, "4 Baseball/Softball Hitting Drills for Proper Bat Path", "Softball", "Timing", "https://www.youtube.com/embed/4Jnd8N9Lwv4" },
                    { 298, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for softball players.", 3, 12, "Optimal Posture for a Hitter — The Hitting Vault", "Softball", "Timing", "https://www.youtube.com/embed/U72mLOkvqrk" },
                    { 299, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for softball players.", 4, 12, "Top-Hand Drill for an Optimal Bat Path", "Softball", "Timing", "https://www.youtube.com/embed/W2-kOnlmbY0" },
                    { 300, "Hitting", new DateTime(2026, 7, 30, 20, 0, 0, 0, DateTimeKind.Utc), "Advanced timing drill for softball players.", 5, 12, "Excellent Hand-Path Drill for an Elite Compact Swing", "Softball", "Timing", "https://www.youtube.com/embed/KhyK-laglXc" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drills_Sport",
                table: "Drills",
                column: "Sport");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_DrillId_LogDate",
                table: "ProgressLogs",
                columns: new[] { "DrillId", "LogDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DrillId",
                table: "Schedules",
                column: "DrillId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressLogs");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Drills");
        }
    }
}
