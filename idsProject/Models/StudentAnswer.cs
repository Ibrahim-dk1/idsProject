namespace Ids.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; } = false;
        public string? WrittenAnswer { get; set; }

        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int? SelectedAnswerId { get; set; }
        public Answer? SelectedAnswer { get; set; }
    }

}
