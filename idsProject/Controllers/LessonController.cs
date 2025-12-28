using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ids.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LessonController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lessons
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lessons = await _context.Lessons
                .Select(l => new LessonResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Order = l.Order,
                    EstimatedDuration = l.EstimatedDuration,
                    CourseId = l.CourseId
                })
                .ToListAsync();

            return Ok(lessons);
        }

        // GET: api/Lessons/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            return Ok(lesson);
        }

        // POST: api/Lessons
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonDto dto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();



            var lesson = new Lesson
            {
                Title = dto.Title,
                Content = dto.Content,
                VideoUrl = dto.VideoUrl,
                Order = dto.Order,
                EstimatedDuration = dto.EstimatedDuration,
                CourseId = dto.CourseId,
                CreatedBy = userId
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }

        // PUT: api/Lessons/5
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLessonDto dto)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            lesson.Title = dto.Title;
            lesson.Content = dto.Content;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.Order = dto.Order;
            lesson.EstimatedDuration = dto.EstimatedDuration;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Lessons/5
        [Authorize(Roles = "Instructor,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
