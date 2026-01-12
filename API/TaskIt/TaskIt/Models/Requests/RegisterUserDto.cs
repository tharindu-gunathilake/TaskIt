using System.ComponentModel.DataAnnotations;

namespace TaskIt.Models.Requests
{
    public class RegisterUserDto
    {
        [Required, MinLength(6), MaxLength(12)]
        public required string UserName { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required, MinLength(8)]
        public required string Password { get; set; }
    }
}
