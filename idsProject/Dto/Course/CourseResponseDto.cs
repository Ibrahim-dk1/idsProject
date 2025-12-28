namespace idsProject.Dtos.Course
{
    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public int CourseDuration { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }= null!;
    }
}
