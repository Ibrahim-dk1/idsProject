namespace Ids.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PassingScore { get; set; }
        public int? TimeLimit { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }

}
