using Ids.Models;
using idsProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ids.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Answer> Answers { get; set; } = null!;
        public DbSet<Certificate> Certificates { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<LessonCompletion> LessonCompletions { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        public DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        public DbSet<StudentAnswer> StudentAnswers { get; set; } = null!;

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // ✅ ADDED (already existed, just confirming)
        public DbSet<CourseRating> CourseRatings => Set<CourseRating>();
        public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // User (Creator) -> Course
            // =========================
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Creator)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // User (Creator) -> Lesson
            // =========================
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.User)
                .WithMany(u => u.Lessons)
                .HasForeignKey(l => l.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Course -> Lesson
            // =========================
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Certificate -> Course
            // =========================
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Course)
                .WithMany(co => co.Certificates)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Certificate -> User
            // =========================
            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Course -> Quiz
            // =========================
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Lesson -> Quiz (optional)
            // =========================
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.Quizzes)
                .HasForeignKey(q => q.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Quiz -> Question
            // =========================
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Question -> Answer
            // =========================
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // User -> QuizAttempt
            // =========================
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Quiz -> QuizAttempt
            // =========================
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // QuizAttempt -> StudentAnswer
            // =========================
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.QuizAttempt)
                .WithMany(qa => qa.StudentAnswers)
                .HasForeignKey(sa => sa.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // StudentAnswer -> Question
            // =========================
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // StudentAnswer -> Answer (optional)
            // =========================
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.SelectedAnswer)
                .WithMany()
                .HasForeignKey(sa => sa.SelectedAnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // User -> LessonCompletion
            // =========================
            modelBuilder.Entity<LessonCompletion>()
                .HasOne(lc => lc.User)
                .WithMany(u => u.LessonCompletions)
                .HasForeignKey(lc => lc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Lesson -> LessonCompletion
            // =========================
            modelBuilder.Entity<LessonCompletion>()
                .HasOne(lc => lc.Lesson)
                .WithMany(l => l.LessonCompletions)
                .HasForeignKey(lc => lc.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================================
            // ✅ NEW: CourseRating relations
            // =====================================================
            modelBuilder.Entity<CourseRating>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.Ratings)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseRating>()
                .HasOne(cr => cr.User)
                .WithMany()
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One rating per user per course
            modelBuilder.Entity<CourseRating>()
                .HasIndex(cr => new { cr.CourseId, cr.UserId })
                .IsUnique();

            // =====================================================
            // ✅ NEW: LessonProgress relations
            // =====================================================
            modelBuilder.Entity<LessonProgress>()
                .HasOne(lp => lp.Course)
                .WithMany()
                .HasForeignKey(lp => lp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

          

            modelBuilder.Entity<LessonProgress>()
                .HasOne(lp => lp.User)
                .WithMany()
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // One progress record per user per lesson
            modelBuilder.Entity<LessonProgress>()
                .HasIndex(lp => new { lp.UserId, lp.LessonId })
                .IsUnique();
        }
    }
}
