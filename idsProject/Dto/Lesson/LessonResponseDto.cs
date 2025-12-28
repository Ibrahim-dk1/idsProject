namespace idsProject.Dtos.Lesson
{
    public class LessonResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int Order { get; set; }
        public int EstimatedDuration { get; set; }
        public int CourseId { get; set; }
    }
}
