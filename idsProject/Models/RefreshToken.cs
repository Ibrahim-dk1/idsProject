using Ids.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace idsProject.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

       
        [Required]
        public string TokenHash { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        
        public DateTime ExpiresAt { get; set; }

        
        public bool IsRevoked { get; set; } = false;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByIp { get; set; }


        public string? ReplacedByTokenHash { get; set; }
    }
}
