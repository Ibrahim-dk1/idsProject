namespace Ids.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.Now;

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }

}
