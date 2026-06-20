using System.Text.Json.Serialization.Metadata;

namespace Training_Api.DtoModels
{
    public class WorkoutExerciseShortDto
    {
        public string? Name { get; set; }

        public int? Weight { get; set; }

        public int Repetitions { get; set; }
    }
}
