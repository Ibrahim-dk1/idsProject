using AutoMapper;
using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Quiz;
using idsProject.Dtos.QuizAttemptDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizAttemptController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public QuizAttemptController(IMapper mapper,AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizAttemptResponse>>> GetAll()
        {
            var quizList = await _context.QuizAttempts.ToListAsync();

            var mappedResults = _mapper.Map<IEnumerable<QuizAttemptResponse>>(quizList);

            return Ok(mappedResults);
        }
        [Authorize(Roles = "Student")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<QuizAttemptResponse>>> GetById(int id)
        {
            var quizAttempt = await _context.QuizAttempts.FindAsync(id);
            var mappedResults = _mapper.Map<IEnumerable<QuizAttemptResponse>>(quizAttempt);

            return Ok(mappedResults);
        }
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<ActionResult<QuizAttemptResponse>> Create(QuizAttemptCreate dto)
        {
            // 1. SAFETY CHECK: Verify the Quiz and User exist in the database
            var quizExists = await _context.Quizzes.AnyAsync(q => q.Id == dto.QuizId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);

            if (!quizExists || !userExists)
            {
                // Return 400 Bad Request with a helpful message
                return BadRequest(new
                {
                    message = "Invalid reference IDs provided.",
                    quizFound = quizExists,
                    userFound = userExists
                });
            }

            
            var quizAttempt = _mapper.Map<QuizAttempt>(dto);

            await _context.QuizAttempts.AddAsync(quizAttempt);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<QuizAttemptResponse>(quizAttempt);

            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<QuizAttemptResponse>> Update(QuizAttemptUpdate dto,int id)
        {
            var existingAttempt = await _context.QuizAttempts.FindAsync(id);
            if (existingAttempt == null) return NotFound();

            _mapper.Map( dto,existingAttempt);
           
            await _context.SaveChangesAsync();

            var response = _mapper.Map<QuizAttemptResponse>(existingAttempt);
            return Ok(response);
            
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<QuizAttemptResponse>> Delete(int id)
        {
            
            var quizattempt =await _context.QuizAttempts.FindAsync(id);
            if(quizattempt == null) return NotFound();
            _context.QuizAttempts.Remove(quizattempt);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<QuizAttemptResponse>(quizattempt);
            return Ok(response);
        }
    }

}
