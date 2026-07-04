using Training_Api.Enums;

namespace Training_Api.DtoModels
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public Role Role { get; set; }

        public List<WorkoutReadDto> Workouts { get; set; } = new();
    }
}
