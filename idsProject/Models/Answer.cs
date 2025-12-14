namespace Ids.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = null!;
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }

}
