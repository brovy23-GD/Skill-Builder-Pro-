using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    public partial class AddDrillGroupAndStringDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DrillGroup",
                table: "Drills",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Duration",
                table: "Drills",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrillGroup",
                table: "Drills");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "Drills",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}