using AutoMapper;
using Ids.Data;
using Ids.Models;
using idsProject.Dtos.StudentAnswer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAnswerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudentAnswerController(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentAnswerResponseDto>>> GetAll()
        {
            var studentAnswers = await _context.StudentAnswers.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<StudentAnswerResponseDto>>(studentAnswers));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAnswerResponseDto>> GetById(int id)
        {
            var studentAnswer = await _context.StudentAnswers.FindAsync(id);
            if (studentAnswer == null) return NotFound();

            return Ok(_mapper.Map<StudentAnswerResponseDto>(studentAnswer));
        }

        [HttpPost]
        public async Task<ActionResult<StudentAnswerResponseDto>> Create(StudentAnswerCreateDto dto)
        {
            // 1. Validation: Check if required references exist
            var attemptExists = await _context.QuizAttempts.AnyAsync(a => a.Id == dto.QuizAttemptId);
            var questionExists = await _context.Questions.AnyAsync(q => q.Id == dto.QuestionId);

            if (!attemptExists || !questionExists)
            {
                return BadRequest("Invalid QuizAttemptId or QuestionId provided.");
            }

            // 2. Validation: Check optional SelectedAnswerId if it is provided
            if (dto.SelectedAnswerId.HasValue)
            {
                var answerExists = await _context.Answers.AnyAsync(a => a.Id == dto.SelectedAnswerId);
                if (!answerExists) return BadRequest("The SelectedAnswerId provided does not exist.");
            }

            // 3. Map Dto -> Entity
            var studentAnswer = _mapper.Map<StudentAnswer>(dto);

            // 4. Save to Database
            await _context.StudentAnswers.AddAsync(studentAnswer);
            await _context.SaveChangesAsync();

            // 5. Map Entity -> ResponseDto
            var responseDto = _mapper.Map<StudentAnswerResponseDto>(studentAnswer);

            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentAnswerResponseDto>> Update(int id, StudentAnswerUpdateDto dto)
        {
            var existingRecord = await _context.StudentAnswers.FindAsync(id);
            if (existingRecord == null) return NotFound();

            // Overwrite mapping: Source -> Existing Entity
            _mapper.Map(dto, existingRecord);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<StudentAnswerResponseDto>(existingRecord);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var record = await _context.StudentAnswers.FindAsync(id);
            if (record == null) return NotFound();

            _context.StudentAnswers.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}