using Ids.Data;
using Ids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using idsProject.Dtos.Course;

namespace idsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        // ===============================
        // GET: api/course
        // Get all courses
        // ===============================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAll()
        {
            var courses = await _context.Courses
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    ShortDescription = c.ShortDescription,
                    Difficulty = c.Difficulty,
                    CourseDuration = c.CourseDuration,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy
                })
                .ToListAsync();

            return Ok(courses);
        }

        // ===============================
        // GET: api/course/{id}
        // Get course by id
        // ===============================
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetById(int id)
        {
            var course = await _context.Courses
                .Where(c => c.Id == id)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    ShortDescription = c.ShortDescription,
                    Difficulty = c.Difficulty,
                    CourseDuration = c.CourseDuration,
                    IsPublished = c.IsPublished,
                    CreatedAt = c.CreatedAt,
                    CreatedBy = c.CreatedBy
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound("Course not found");

            return Ok(course);
        }

        // ===============================
        // POST: api/course
        // Add course
        // ===============================
        [HttpPost]
        public async Task<ActionResult> Create(CreateCourseDto dto)
        {
            var course = new Course
            {
                CourseTitle = dto.CourseTitle,
                ShortDescription = dto.ShortDescription,
                FullDescription = dto.FullDescription,
                Difficulty = dto.Difficulty,
                CourseThumbnail = dto.CourseThumbnail,
                CourseDuration = dto.CourseDuration,
                IsPublished = dto.IsPublished,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course.Id);
        }

        // ===============================
        // PUT: api/course/{id}
        // Edit course
        // ===============================
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound("Course not found");

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

        // ===============================
        // DELETE: api/course/{id}
        // Remove course
        // ===============================
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
                return NotFound("Course not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
