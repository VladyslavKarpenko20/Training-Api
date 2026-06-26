using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Workout",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFhiDAh6Z9xHw8f1fYMdAnCTsOb+r1Gcg/sAA0rl0Kk9pWtEep5H/XaZ3Z38fSH1uA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Workout");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAED3PcriBBKbuhbrmlC7yTpa9iDOScUENmqxrHh7QpqRZSAVyZIm/5ynxdOW8g8Hs7w==");
        }
    }
}
