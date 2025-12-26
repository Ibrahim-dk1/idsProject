namespace idsProject.Dtos.Answer
{
    public class AnswerResponseDto
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
