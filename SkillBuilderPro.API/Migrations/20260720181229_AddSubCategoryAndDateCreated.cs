using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBuilderPro.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoryAndDateCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Drills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "Drills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.UpdateData(
                table: "Drills",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "DateCreated", "SubCategory" },
                values: new object[] { new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Drills");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Drills");
        }
    }
}
