namespace idsProject.Dtos.Question
{
    public class QuestionResponseDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = null!;
        public string QuestionType { get; set; } = null!;
        public int QuizId { get; set; }
    }
}
