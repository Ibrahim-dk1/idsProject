namespace idsProject.Dtos.Quiz
{
    public class QuizResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PassingScore { get; set; }
        public int? TimeLimit { get; set; }
        public int CourseId { get; set; }
        public int? LessonId { get; set; }

    }
}
