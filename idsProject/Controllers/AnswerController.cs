using AutoMapper;
using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Answer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AnswerController(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerResponseDto>>> GetAll()
        {
            var answers = await _context.Answers.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AnswerResponseDto>>(answers));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerResponseDto>> GetById(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null) return NotFound();

            return Ok(_mapper.Map<AnswerResponseDto>(answer));
        }

        [HttpPost]
        public async Task<ActionResult<AnswerResponseDto>> Create(AnswerCreateDto dto)
        {
            // SAFETY CHECK: Does the Question exist?
            var questionExists = await _context.Questions.AnyAsync(q => q.Id == dto.QuestionId);
            if (!questionExists)
            {
                return BadRequest($"Cannot create answer: Question with ID {dto.QuestionId} not found.");
            }

            var answer = _mapper.Map<Answer>(dto);

            await _context.Answers.AddAsync(answer);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<AnswerResponseDto>(answer);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AnswerResponseDto>> Update(int id, AnswerUpdateDto dto)
        {
            var existingAnswer = await _context.Answers.FindAsync(id);
            if (existingAnswer == null) return NotFound();

            // Map DTO changes onto the existing tracked entity
            _mapper.Map(dto, existingAnswer);

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<AnswerResponseDto>(existingAnswer));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null) return NotFound();

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}