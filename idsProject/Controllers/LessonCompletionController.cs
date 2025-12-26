using Ids.Data;
using idsProject.Dtos.LessonCompletion;
using Ids.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonCompletionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LessonCompletionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var completions = await _context.LessonCompletions
                .Select(lc => new LessonCompletionResponseDto
                {
                    Id = lc.Id,
                    LessonId = lc.LessonId,
                    UserId = lc.UserId,
                    CompletedDate = lc.CompletedDate
                })
                .ToListAsync();

            return Ok(completions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lc = await _context.LessonCompletions.FindAsync(id);
            if (lc == null) return NotFound();

            var response = new LessonCompletionResponseDto
            {
                Id = lc.Id,
                LessonId = lc.LessonId,
                UserId = lc.UserId,
                CompletedDate = lc.CompletedDate
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonCompletionDto dto)
        {
            var completion = new LessonCompletion
            {
                LessonId = dto.LessonId,
                
            };

            _context.LessonCompletions.Add(completion);
            await _context.SaveChangesAsync();

            var response = new LessonCompletionResponseDto
            {
                Id = completion.Id,
                LessonId = completion.LessonId,
                UserId = completion.UserId,
                CompletedDate = completion.CompletedDate
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var completion = await _context.LessonCompletions.FindAsync(id);
            if (completion == null) return NotFound();

            _context.LessonCompletions.Remove(completion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
