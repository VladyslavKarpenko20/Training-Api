using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Training_Api.DtoModels
{
    public class RegistrDto
    {
        [Required]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
