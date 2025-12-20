namespace idsProject.Dtos.Answer
{
    public class AnswerCreateDto
    {
        public string AnswerText { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
