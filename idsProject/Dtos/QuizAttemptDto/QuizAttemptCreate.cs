

namespace idsProject.Dtos.QuizAttemptDto
{
    public class QuizAttemptCreate
    {
        // The client must tell us which quiz and which user
        public int QuizId { get; set; }
        public int UserId { get; set; }

        // The score achieved
        public int Score { get; set; }

    }
}