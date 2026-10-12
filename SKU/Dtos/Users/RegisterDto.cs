using System.ComponentModel.DataAnnotations;

namespace SKU.Dtos.Users
{
    public class RegisterDto
    {

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        [MaxLength(50)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        
        public int UserTypeId { get; set; }
    }
}
