namespace Training_Api.DtoModels
{
    public class WorkoutWriteDto
    {
        public DateTimeOffset startDate { get; set; }

        public DateTimeOffset endDate { get; set; } 

        public List<WorkoutExerciseWriteDto> Workouts { get; set; } = new();
    }
}
