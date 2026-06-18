using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Training_Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [MinLength(3)]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string Password { get; set; }
    }
}
