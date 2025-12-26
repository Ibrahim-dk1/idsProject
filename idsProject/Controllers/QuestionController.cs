using AutoMapper;
using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public QuestionController(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionResponseDto>>> GetAll()
        {
            var questions = await _context.Questions.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<QuestionResponseDto>>(questions));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionResponseDto>> GetById(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            return Ok(_mapper.Map<QuestionResponseDto>(question));
        }
        [Authorize(Roles = "Instructor,Admin")]
        [HttpPost]
        public async Task<ActionResult< QuestionResponseDto>> Create(QuestionCreateDto dto)
        {
            var quizExists = await _context.Quizzes.AnyAsync(q => q.Id == dto.QuizId);

            if (!quizExists)
            {
                // 404 is technically more accurate, but 400 with a message is also great
                return NotFound($"Request failed: Quiz with ID {dto.QuizId} does not exist.");
            }
            var question = _mapper.Map<Question>(dto);

            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<QuestionResponseDto>(question);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionResponseDto>> Update(int id, QuestionUpdateDto dto)
        {
            var existingQuestion = await _context.Questions.FindAsync(id);
            if (existingQuestion == null) return NotFound();

            // Map DTO onto the existing tracked entity
            _mapper.Map(dto, existingQuestion);

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<QuestionResponseDto>(existingQuestion));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent(); // Usually better for Delete than returning the object
        }
    }
}