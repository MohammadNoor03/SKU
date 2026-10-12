using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Auth
{
    public class LoginDto
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}