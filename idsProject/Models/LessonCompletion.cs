namespace Ids.Models
{
    public class LessonCompletion
    {
        public int Id { get; set; }
        public DateTime CompletedDate { get; set; } = DateTime.Now;

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
