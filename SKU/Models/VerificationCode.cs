using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKU.Models
{
    public enum VerificationPurpose
    {
        PhoneVerification = 1,
        PasswordReset = 2
    }

    public class VerificationCode
    {
        public long Id { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty;

        public VerificationPurpose Purpose { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public int Attempts { get; set; }
        public bool IsUsed { get; set; }
    }
}