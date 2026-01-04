using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace idsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CourseController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ============================
        // GET: api/course
        // ============================
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAll()
        {
            var courses = await _context.Courses
                .Include(c => c.Creator)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    ShortDescription = c.ShortDescription,
                    FullDescription = c.FullDescription,
                    Difficulty = c.Difficulty,
                    CourseDuration = c.CourseDuration,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy ?? "Unknown", // keep the ID
                    CreatorName = c.Creator != null
                        ? $"{c.Creator.FirstName} {c.Creator.LastName}"
                        : "Unknown", // construct display name
                    CourseThumbnail = c.CourseThumbnail
                })
                .ToListAsync();

            return Ok(courses);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Creator)
                .Where(c => c.Id == id)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    ShortDescription = c.ShortDescription,
                    FullDescription = c.FullDescription,
                    Difficulty = c.Difficulty,
                    CourseDuration = c.CourseDuration,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy ?? "Unknown",
                    CreatorName = c.Creator != null
                        ? $"{c.Creator.FirstName} {c.Creator.LastName}"
                        : "Unknown",
                    CourseThumbnail = c.CourseThumbnail
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound("Course not found");

            return Ok(course);
        }

        // ============================
        // POST: api/course
        // ============================
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateCourseDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? videoUrl = null;

            // Handle video upload
            if (dto.IntroVideo != null && dto.IntroVideo.Length > 0)
            {
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadPath = Path.Combine(webRoot, "uploads", "videos");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.IntroVideo.FileName)}";
                var fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.IntroVideo.CopyToAsync(stream);
                }

                videoUrl = $"{Request.Scheme}://{Request.Host}/uploads/videos/{fileName}";
            }

            var course = new Course
            {
                CourseTitle = dto.CourseTitle,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                Difficulty = dto.Difficulty,
                CourseThumbnail = dto.CourseThumbnail,
                IntroVideoUrl = videoUrl,
                CourseDuration = dto.CourseDuration,
                IsPublished = dto.IsPublished,
                CreatedBy = userId ?? "Unknown",
                CreatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = course.Id },
                new
                {
                    course.Id,
                    VideoUrl = videoUrl
                }
            );
        }

        // ============================
        // PUT: api/course/{id}
        // ============================
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.CourseTitle = dto.CourseTitle;
            course.ShortDescription = dto.ShortDescription;
            course.FullDescription = dto.FullDescription;
            course.Difficulty = dto.Difficulty;
            course.CourseThumbnail = dto.CourseThumbnail;
            course.CourseDuration = dto.CourseDuration;
            course.IsPublished = dto.IsPublished;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ============================
        // DELETE: api/course/{id}
        // ============================
        [Authorize(Roles = "Instructor,Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
