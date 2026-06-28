using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEndDateForWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Workout",
                newName: "startDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "endDate",
                table: "Workout",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEI84EGA3EtT8gzw13LoEtIe1v0MGo5TpR5CMLgKJU5jU/26X+vt/fW2QjJLKcEA+5Q==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDate",
                table: "Workout");

            migrationBuilder.RenameColumn(
                name: "startDate",
                table: "Workout",
                newName: "Date");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFhiDAh6Z9xHw8f1fYMdAnCTsOb+r1Gcg/sAA0rl0Kk9pWtEep5H/XaZ3Z38fSH1uA==");
        }
    }
}
