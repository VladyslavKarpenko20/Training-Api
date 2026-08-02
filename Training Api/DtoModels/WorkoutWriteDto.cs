namespace Training_Api.DtoModels
{
    public class WorkoutWriteDto
    {
        public DateTimeOffset StartDate { get; init; }

        public DateTimeOffset EndDate { get; init; } 

        public List<WorkoutExerciseWriteDto> WorkoutsExercise { get; init; } = new();
    }
}
