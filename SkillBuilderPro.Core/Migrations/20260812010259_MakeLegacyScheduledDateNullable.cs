using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.Core.Migrations
{
    /// <inheritdoc />
    public partial class MakeLegacyScheduledDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<System.DateTime>(
                name: "ScheduledDate",
                table: "Schedules",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(System.DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<System.DateTime>(
                name: "ScheduledDate",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(System.DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
