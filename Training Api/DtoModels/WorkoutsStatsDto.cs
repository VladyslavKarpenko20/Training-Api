using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Training_Api.DtoModels
{
    public class WorkoutsStatsDto
    {
        public int TotalWorkout { get; set; }

        public int TotalWorkoutExercise { get; set; }

        public int TotalComplateWorkout { get; set; }

        public int TotalCanceledWorkout {get; set;}

        public int TotalInProgresWorkout { get; set; }

        public int TotalPlannedWorkout { get; set; }

        public double AvarageExercisPerWorkout { get; set; }

        public int? MaxWeight { get; set; }

        public string? MostCommonExercise { get; set; }

    }
}
