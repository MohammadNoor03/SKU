using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKU.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;


        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsVerifiedByEmail { get; set; }
        public bool IsVerifiedByPhone { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("UserType_Lookup")]
        public long UserTypeId { get; set; }
        public UserType_Lookup UserType_Lookup { get; set; } = null!;

        public ICollection<UserLastLogins> LastLogins { get; set; } = new List<UserLastLogins>();
    }
}

