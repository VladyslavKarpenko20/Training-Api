namespace Training_Api.Models
{
    public class Workout
    {
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public List<WorkoutExercise> WorkoutExercise { get; set; } = new();
    }
}
