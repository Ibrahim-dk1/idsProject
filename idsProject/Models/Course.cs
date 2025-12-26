namespace Ids.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string FullDescription { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public string? CourseThumbnail { get; set; }
        public int CourseDuration { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = null!;
        public User Creator { get; set; } = null!;

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
