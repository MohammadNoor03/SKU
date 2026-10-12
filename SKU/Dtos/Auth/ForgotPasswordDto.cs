using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}