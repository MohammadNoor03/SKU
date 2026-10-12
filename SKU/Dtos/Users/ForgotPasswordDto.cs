using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Users
{
    public class ForgotPasswordDto
    {
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
