using System.ComponentModel.DataAnnotations;

namespace SKU.Models
{
    public class UserType_Lookup
    {
        public long Id { get; set; }
        public int MajorCode { get; set; }
        public int MinorCode { get; set; }


        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
