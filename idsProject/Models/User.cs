using Microsoft.AspNetCore.Identity;

namespace Ids.Models
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; } = null!;
       
        public string LastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public ICollection<LessonCompletion> LessonCompletions { get; set; } = new List<LessonCompletion>();
    }

}
