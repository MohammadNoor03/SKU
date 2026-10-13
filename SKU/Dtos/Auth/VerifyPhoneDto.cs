using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Auth
{
    public class VerifyPhoneDto
    {
        [Required]
        [StringLength(5, MinimumLength = 5)]
        public string Code { get; set; } = string.Empty;
    }
}