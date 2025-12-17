namespace idsProject.Dtos.Course
{
    public class CreateCourseDto
    {
        public string CourseTitle { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string FullDescription { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public string? CourseThumbnail { get; set; }
        public int CourseDuration { get; set; }
        public bool IsPublished { get; set; }
        public int CreatedBy { get; set; }
    }
}
