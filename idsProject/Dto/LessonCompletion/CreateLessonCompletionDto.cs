namespace idsProject.Dtos.LessonCompletion
{
    public class CreateLessonCompletionDto
    {
        public int LessonId { get; set; }
        public string UserId { get; set; } = null!;
    }
}
