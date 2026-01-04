using Ids.Models;

namespace idsProject.Models
{
    public class CourseRating
    {
        public int Id { get; set; }

        // Relations
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;

        // Rating data
        public int Rating { get; set; } // 1 to 5
        public string? Review { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
