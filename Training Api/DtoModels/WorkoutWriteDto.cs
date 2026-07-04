namespace Training_Api.DtoModels
{
    public class WorkoutWriteDto
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; } 

        public List<WorkoutExerciseWriteDto> WorkoutsExercise { get; set; } = new();
    }
}
