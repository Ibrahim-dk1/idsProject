namespace Ids.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string DownloadUrl { get; set; } = null!;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

}
