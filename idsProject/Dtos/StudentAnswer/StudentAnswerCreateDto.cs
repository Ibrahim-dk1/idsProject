namespace idsProject.Dtos.StudentAnswer
{
    public class StudentAnswerCreateDto
    {
        public string? WrittenAnswer { get; set; }
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedAnswerId { get; set; }
    }
}
