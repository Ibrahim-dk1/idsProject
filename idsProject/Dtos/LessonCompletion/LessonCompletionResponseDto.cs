namespace idsProject.Dtos.LessonCompletion
{
    public class LessonCompletionResponseDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int UserId { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
