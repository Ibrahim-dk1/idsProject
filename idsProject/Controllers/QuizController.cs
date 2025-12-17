using Ids.Data;
using Ids.Models;
using idsProject.Dtos.Quiz;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace idsProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {


        private readonly AppDbContext _context;

        public QuizController(AppDbContext context) { _context = context; }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetAll()
        {
            var quizzes = await _context.Quizzes
                .Select(Q => new QuizResponseDto
                {
                    Id = Q.Id,
                    Title = Q.Title,
                    PassingScore = Q.PassingScore,
                    TimeLimit = Q.TimeLimit,
                    CourseId = Q.CourseId,
                    LessonId = Q.LessonId

                }).ToListAsync();

            return Ok(quizzes);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<QuizResponseDto>> GetById(int id)
        {
            var Quiz = await _context.Quizzes.Where(Q => Q.Id == id).Select(Q => new QuizResponseDto
            {
                Id = Q.Id,
                Title = Q.Title,
                PassingScore = Q.PassingScore,
                TimeLimit = Q.TimeLimit,
                CourseId = Q.CourseId,
                LessonId = Q.LessonId
            }).FirstOrDefaultAsync();
            return Ok(Quiz);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var Quiz = await _context.Quizzes.FindAsync(id);
            if (Quiz == null) return NotFound("Quiz not found");

            _context.Quizzes.Remove(Quiz);

            return Ok(Quiz.Id);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateQuizDto dto)
        {
            var Quiz = new Quiz
            {
                Title = dto.Title,
                PassingScore = dto.PassingScore,
                TimeLimit = dto.TimeLimit,
                CourseId = dto.CourseId,
                LessonId = dto.LessonId

            };
            _context.Quizzes.Add(Quiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = Quiz.Id }, Quiz.Id);


        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateQuizDto dto)
        {
            var Quiz = await _context.Quizzes.FindAsync(id);

            var quiz = new Quiz
            {
                Title = dto.Title,
                PassingScore = dto.PassingScore,
                TimeLimit = dto.TimeLimit


            };
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return Ok(quiz.Id);

        }



    }
}
