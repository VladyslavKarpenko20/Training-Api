using System.ComponentModel.DataAnnotations;

namespace Training_Api.DtoModels
{
    public class LoginDto
    {
        [Required]    
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }
}
