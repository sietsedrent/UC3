using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UC3.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseModels",
                columns: table => new
                {
                    exerciseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    exerciseName = table.Column<string>(type: "TEXT", nullable: false),
                    muscleGroup = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseModels", x => x.exerciseId);
                });

            migrationBuilder.CreateTable(
                name: "TrainingDataModels",
                columns: table => new
                {
                    TrainingDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    workoutId = table.Column<int>(type: "INTEGER", nullable: false),
                    exerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    amountOfSets = table.Column<int>(type: "INTEGER", nullable: false),
                    amountOfReps = table.Column<int>(type: "INTEGER", nullable: false),
                    liftedWeight = table.Column<float>(type: "REAL", nullable: false),
                    e1RM = table.Column<float>(type: "REAL", nullable: false),
                    pr = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingDataModels", x => x.TrainingDataId);
                });

            migrationBuilder.CreateTable(
                name: "UserModels",
                columns: table => new
                {
                    userId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    password = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    bio = table.Column<string>(type: "TEXT", nullable: false),
                    profilepicture = table.Column<string>(type: "TEXT", nullable: false),
                    RegisteryDate = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModels", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutModels",
                columns: table => new
                {
                    workoutId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userId = table.Column<int>(type: "INTEGER", nullable: false),
                    workoutDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    typeWorkout = table.Column<string>(type: "TEXT", nullable: false),
                    comments = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutModels", x => x.workoutId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseModels");

            migrationBuilder.DropTable(
                name: "TrainingDataModels");

            migrationBuilder.DropTable(
                name: "UserModels");

            migrationBuilder.DropTable(
                name: "WorkoutModels");
        }
    }
}
