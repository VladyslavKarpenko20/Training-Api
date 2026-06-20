using Training_Api.Models;

namespace Training_Api.DtoModels
{
    public class WorkoutReadDto
    {
        public int Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public int UserId { get; set; }

        public List<WorkoutExerciseShortDto> WorkoutExerciseShort { get; set; } = new();
    }
}
