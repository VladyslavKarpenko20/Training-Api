namespace Training_Api.DtoModels
{
    public class WorkoutUpdateDto
    {
        public DateTimeOffset Date { get; set; }

        public List<WorkoutExerciseShortDto> ExerciseShorts { get; set; }

    }
}
