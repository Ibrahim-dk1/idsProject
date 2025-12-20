namespace idsProject.Dtos.StudentAnswer
{
    public class StudentAnswerResponseDto
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public string? WrittenAnswer { get; set; }
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedAnswerId { get; set; }
    }
}