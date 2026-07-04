using Training_Api.Enums;

namespace Training_Api.Models
{
    public class Workout
    {
        public int Id { get; init; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public Status Status { get; set; }

        public int UserId { get; init; }

        public User? User { get; init; }

        public List<WorkoutExercise> WorkoutExercise { get; init; } = new();
    }
}
