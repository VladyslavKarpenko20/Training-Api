using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEAvYnDmmRCO3rSEmrKJh/OMMua3UO2afisfBycUZDY5i5QTvKoloqXCSvHnHpX1zTA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHwXF4y6i+V9xUyhn4kKd9RgTkq0Nx2Gd5IasSxgZqWH/d3Kt84Cp5pw97A2zqacHA==");
        }
    }
}
