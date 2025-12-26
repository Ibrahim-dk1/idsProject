namespace idsProject.Dtos.LessonCompletion
{
    public class LessonCompletionResponseDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime CompletedDate { get; set; }
    }
}
