using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string NewPassword { get; set; } = string.Empty;
    }
}