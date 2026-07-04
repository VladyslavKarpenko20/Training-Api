using System.ComponentModel.DataAnnotations;

namespace Training_Api.Models
{
    public class WorkoutExercise
    {
        public int Id { get; init; }

        public Workout? Workout { get; init; }

        public int WorkoutId { get; init; }

        [MaxLength(20)]
        public string? Name { get; set; }

        public int? Weight { get; set; }

        public int Repetitions { get; set; }


    }
}
