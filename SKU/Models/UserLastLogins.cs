using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKU.Models
{
    public class UserLastLogins
    {
        public long Id { get; set; }



        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        public DateTime LastLoginTime { get; set; } = DateTime.UtcNow;
    }
}
