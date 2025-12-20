namespace idsProject.Dtos.Lesson
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int EstimatedDuration { get; set; }
        public int CourseId { get; set; }
        public int CreatedBy { get; set; }
    }
}
