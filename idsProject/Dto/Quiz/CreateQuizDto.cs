namespace idsProject.Dtos.Quiz
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = null!;
        public int PassingScore { get; set; }
        public int? TimeLimit { get; set; }

        public int CourseId { get; set; }

        public int? LessonId { get; set; }


    }
}
