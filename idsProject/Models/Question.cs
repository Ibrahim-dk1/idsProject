namespace Ids.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = null!;
        public string QuestionType { get; set; } = null!;

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }

}
