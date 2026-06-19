using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "Password", "Role" },
                values: new object[] { 10, "Admin@gmail.com", "Admin", "AQAAAAIAAYagAAAAENCzE6R6qkvUFJ1WoFx8p1KSHhkHVhtriqTQ9/skux2U6pALa2bP0qM3aa37OExebg==", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "Password", "Role" },
                values: new object[] { 1, "Admin@gmail.com", "Admin", "AQAAAAIAAYagAAAAEC5qPNZm8s4NC0ybqIbIyIc44t3EeDxXOrt4PyvO22xLgE/kpiUXUQFAXWgalsQM4Q==", 1 });
        }
    }
}
