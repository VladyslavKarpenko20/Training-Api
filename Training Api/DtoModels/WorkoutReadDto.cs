using Training_Api.Enums;
using Training_Api.Models;

namespace Training_Api.DtoModels
{
    public class WorkoutReadDto
    {
        public int Id { get; set; }

        public DateTimeOffset startDate { get; set; }
        
        public DateTimeOffset endDate { get; set; }

        public Status Status { get; set; }

        public int UserId { get; set; }

        public List<WorkoutExerciseShortDto> WorkoutExerciseShort { get; set; } = new();
    }
}
