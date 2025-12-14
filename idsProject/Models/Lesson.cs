namespace Ids.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int EstimatedDuration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int CreatedBy { get; set; }
        public User User { get; set; } = null!;

        public ICollection<LessonCompletion> LessonCompletions { get; set; } = new List<LessonCompletion>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }

}
