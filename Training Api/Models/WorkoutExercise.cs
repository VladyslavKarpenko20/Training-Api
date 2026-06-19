namespace Training_Api.Models
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        public Workout? Workout { get; set; }

        public int WorkoutId { get; set; }

        public string? Name { get; set; }

        public int? Weight { get; set; }

        public int Repetitions { get; set; }


    }
}
