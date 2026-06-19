using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHwXF4y6i+V9xUyhn4kKd9RgTkq0Nx2Gd5IasSxgZqWH/d3Kt84Cp5pw97A2zqacHA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFopkl2Ivub7hVFeFOKfKLWNCrCdiMlEUlsc3kmKZRXtsOLU0aiJs+CntJ1ab5vrtQ==");
        }
    }
}
