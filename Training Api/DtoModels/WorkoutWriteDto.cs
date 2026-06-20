namespace Training_Api.DtoModels
{
    public class WorkoutWriteDto
    {
        public DateTimeOffset Date { get; set; }

        public List<WorkoutExerciseShortDto> Workouts { get; set; } = new();
    }
}
