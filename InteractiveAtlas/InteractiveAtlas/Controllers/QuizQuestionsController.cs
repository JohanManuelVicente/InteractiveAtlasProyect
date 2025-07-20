using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Entities;
using System.Xml.Linq;
using InteractiveAtlas.Data;
using InteractiveAtlas.DTOs;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizQuestionsController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;

        public QuizQuestionsController(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetQuizQuestions()
        {
            var quizQuestions = _context.QuizQuestions.ToList();
            return Ok(quizQuestions);
        }

        [HttpGet("{id}")]
        public IActionResult GetQuizQuestionById(int id)
        {
            var quizQuestion = _context.QuizQuestions.FirstOrDefault(q => q.Id == id);

            if (quizQuestion == null)
            {
                return BadRequest($"Quiz Question with ID: {id} not found");
            }

            return Ok(quizQuestion);
        }

        [HttpPost]
        public IActionResult CreateQuizQuestion([FromBody] QuizQuestionDto request)
        {
            if (request == null)
            {
                return BadRequest("The QuizQuestion cannot be null");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
                }
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("El texto de la pregunta es requerido");
            }

            var quizQuestion = new QuizQuestion
            {
                Text = request.Text,
                DifficultyLevel = request.DifficultyLevel,
                ProvinceId = request.ProvinceId,
            };

            _context.QuizQuestions.Add(quizQuestion);
            _context.SaveChanges();
            return Ok(new { id = quizQuestion.Id });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateQuizQuestion(int id, [FromBody] QuizQuestionDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                return BadRequest("El texto de la pregunta es nulo");
            }

            var existingQuizQuestion = _context.QuizQuestions.FirstOrDefault(qq => qq.Id == request.Id);
            if (existingQuizQuestion == null)
            {
                return NotFound($"La pregunta con ID {request.Id} no fue encontrada");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
                }
            }

            existingQuizQuestion.Text = request.Text;
            existingQuizQuestion.DifficultyLevel = request.DifficultyLevel;
            existingQuizQuestion.ProvinceId = request.ProvinceId;

            _context.QuizQuestions.Update(existingQuizQuestion);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuizQuestion(int id)
        {
            var quizQuestion = _context.QuizQuestions.FirstOrDefault(q => q.Id == id);
            if (quizQuestion == null)
            {
                return NotFound($"QuizQuestion con ID: {id} no fue encontrada");
            }

            _context.QuizQuestions.Remove(quizQuestion);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
