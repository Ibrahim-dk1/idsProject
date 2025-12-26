namespace idsProject.Dtos.QuizAttemptDto
{
    public class QuizAttemptResponse
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.Now;
        public int QuizId { get; set; }
        public string UserId { get; set; } = null!;


    }
}
