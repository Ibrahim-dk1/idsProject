

namespace idsProject.Dtos.QuizAttemptDto
{
    public class QuizAttemptCreate
    {
        // The client must tell us which quiz and which user
        public int QuizId { get; set; }
        public string UserId { get; set; } = null!;

        // The score achieved
        public int Score { get; set; }

    }
}