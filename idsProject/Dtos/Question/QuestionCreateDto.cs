namespace idsProject.Dtos.Question
{
    public class QuestionCreateDto
    {
        public string QuestionText { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int QuizId { get; set; }
    }
}
