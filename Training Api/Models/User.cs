
using System.ComponentModel.DataAnnotations;
using Training_Api.Enums;

namespace Training_Api.Models
{
    public class User
    {
        public int Id { get; init; }
        
        [MaxLength(20)]
        public string? Name { get; init; }

        [MaxLength(30)]
        public string? Email { get; init; }

        [MaxLength(250)]
        public string? Password { get; set; }

        public List<Workout> Workouts { get; init; } = new();

        public Role Role { get; set; }
    }
}
