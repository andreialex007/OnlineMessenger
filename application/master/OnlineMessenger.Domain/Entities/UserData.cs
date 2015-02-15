using System.ComponentModel.DataAnnotations;

namespace OnlineMessenger.Domain.Entities
{
    public class UserData
    {
        [Required]
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }
        public byte[] AvatarImage { get; set; }
    }
}
