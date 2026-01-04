namespace idsProject.Dtos.Course
{
    public class CreateCourseDto
    {
        public string CourseTitle { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string FullDescription { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public string? CourseThumbnail { get; set; } // Link from frontend
        public int CourseDuration { get; set; }
        public bool IsPublished { get; set; }

        // This handles the actual video file upload
        public IFormFile? IntroVideo { get; set; }
    }
}