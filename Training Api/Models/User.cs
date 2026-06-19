
namespace Training_Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public List<Workout> Workouts { get; set; } = new();

        public Role.Role Role { get; set; }
    }
}
