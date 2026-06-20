using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Workout_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises");

            migrationBuilder.RenameTable(
                name: "WorkoutExercises",
                newName: "WorkoutExercise");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercise",
                newName: "IX_WorkoutExercise_WorkoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercise",
                table: "WorkoutExercise",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAED3PcriBBKbuhbrmlC7yTpa9iDOScUENmqxrHh7QpqRZSAVyZIm/5ynxdOW8g8Hs7w==");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercise_Workout_WorkoutId",
                table: "WorkoutExercise",
                column: "WorkoutId",
                principalTable: "Workout",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercise_Workout_WorkoutId",
                table: "WorkoutExercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercise",
                table: "WorkoutExercise");

            migrationBuilder.RenameTable(
                name: "WorkoutExercise",
                newName: "WorkoutExercises");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercise_WorkoutId",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_WorkoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 10,
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEVazwZ0f0gsRN0GJGZ6vcFAYfex62n+oR4qYQZP6A7NTbSpjDQMbSkl5zFBdA7ivg==");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Workout_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalTable: "Workout",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
