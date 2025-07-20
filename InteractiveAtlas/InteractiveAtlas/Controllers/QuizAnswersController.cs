using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Entities;
using System.Xml.Linq;
using InteractiveAtlas.Data;
using InteractiveAtlas.DTOs;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAnswersController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;

        public QuizAnswersController(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetQuizAnswers()
        {
            var quizAnswers = _context.QuizAnswers.ToList();
            var quizAnswersResponse = new List<QuizAnswerDto>();
            quizAnswersResponse = quizAnswers.Select(a => new QuizAnswerDto
            {
                Id = a.Id,
                Text = a.Text,
                IsCorrect = a.IsCorrect,
                QuestionId = a.QuestionId
            }).ToList();
            return Ok(quizAnswersResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetQuizAnswerById(int id)
        {
            var quizAnswer = _context.QuizAnswers.FirstOrDefault(a => a.Id == id);

            if (quizAnswer == null)
            {
                return BadRequest($"Quiz Answer with ID: {id} not found");
            }

            var quizAnswerResponse = new QuizAnswerDto
            {
                Id = quizAnswer.Id,
                Text = quizAnswer.Text,
                IsCorrect = quizAnswer.IsCorrect,
                QuestionId = quizAnswer.QuestionId
            };
            return Ok(quizAnswerResponse);
        }

        [HttpPost]
        public IActionResult CreateQuizAnswer([FromBody] QuizAnswerDto request)
        {
            if (request == null)
            {
                return BadRequest("The QuizAnswer cannot be null");
            }

            var questionExists = _context.QuizQuestions.Any(q => q.Id == request.QuestionId);
            if (!questionExists)
            {
                return BadRequest($"La pregunta con ID {request.QuestionId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return BadRequest("El texto de la respuesta es requerido");
            }

            var quizAnswer = new QuizAnswer
            {
                Text = request.Text,
                IsCorrect = request.IsCorrect,
                QuestionId = request.QuestionId,
            };

            _context.QuizAnswers.Add(quizAnswer);
            _context.SaveChanges();
            return Ok(new { id = quizAnswer.Id });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateQuizAnswer(int id, [FromBody] QuizAnswerDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                return BadRequest("El texto de la respuesta es nulo");
            }

            var existingQuizAnswer = _context.QuizAnswers.FirstOrDefault(qa => qa.Id == request.Id);
            if (existingQuizAnswer == null)
            {
                return NotFound($"La respuesta con ID {request.Id} no fue encontrada");
            }

            var questionExists = _context.QuizQuestions.Any(q => q.Id == request.QuestionId);
            if (!questionExists)
            {
                return BadRequest($"La pregunta con ID {request.QuestionId} no existe");
            }

            existingQuizAnswer.Text = request.Text;
            existingQuizAnswer.IsCorrect = request.IsCorrect;
            existingQuizAnswer.QuestionId = request.QuestionId;

            _context.QuizAnswers.Update(existingQuizAnswer);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteQuizAnswer(int id)
        {
            var quizAnswer = _context.QuizAnswers.FirstOrDefault(a => a.Id == id);
            if (quizAnswer == null)
            {
                return NotFound($"QuizAnswer con ID: {id} no fue encontrada");
            }

            _context.QuizAnswers.Remove(quizAnswer);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
