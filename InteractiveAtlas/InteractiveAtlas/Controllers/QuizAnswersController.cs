using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.DTOs;
using InteractiveAtlas.Infrastucture.Repositories;
using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Contracts;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAnswersController : ControllerBase
    {
        private readonly  IUnitOfWork _unitOfWork;

        public QuizAnswersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizAnswers()
        {
            return Ok(await _unitOfWork.QuizAnswers.GetAllAsync());
        }

        [HttpGet]
        [Route("with-question")]
        public async Task<IActionResult> GetQuizAnswersWithQuestion()
        {
            var quizAnswers = await _unitOfWork.QuizAnswers.GetAllQuizAnswerWithQuestionAsync();

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
        public async Task<IActionResult> GetQuizAnswerById(int id)
        {
            var quizAnswer = await _unitOfWork.QuizAnswers.GetQuizAnswerWithQuizQuestionByIdAsync(id);

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

        [HttpGet]
        [Route("by-question")]
        public async Task<IActionResult> GetQuizAnswersByQuestionId([FromQuery] int questionId)
        {
            return Ok(await _unitOfWork.QuizAnswers.GetAllQuizAnswerByQuestionIdAsync(questionId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuizAnswer([FromBody] QuizAnswerDto request)
        {
            if (request == null)
            {
                return BadRequest("The QuizAnswer cannot be null");
            }

            var questionExists = _unitOfWork.Context.QuizQuestions.Any(q => q.Id == request.QuestionId);
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

            quizAnswer = await _unitOfWork.QuizAnswers.AddAsync(quizAnswer);
            await _unitOfWork.CompleteAsync();
            return Ok(new { id = quizAnswer.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizAnswer(int id, [FromBody] QuizAnswerDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                return BadRequest("El texto de la respuesta es nulo");
            }

            var existingQuizAnswer = await _unitOfWork.QuizAnswers.GetQuizAnswerWithQuizQuestionByIdAsync(id);
            if (existingQuizAnswer == null)
            {
                return NotFound($"La respuesta con ID {request.Id} no fue encontrada");
            }

            var questionExists = _unitOfWork.Context.QuizQuestions.Any(q => q.Id == request.QuestionId);
            if (!questionExists)
            {
                return BadRequest($"La pregunta con ID {request.QuestionId} no existe");
            }

            existingQuizAnswer.Text = request.Text;
            existingQuizAnswer.IsCorrect = request.IsCorrect;
            existingQuizAnswer.QuestionId = request.QuestionId;

            _unitOfWork.QuizAnswers.UpdateAsync(existingQuizAnswer).Wait();
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAnswer(int id)
        {
            var quizAnswer = await _unitOfWork.QuizAnswers.GetByIdAsync(id);
            if (quizAnswer == null)
            {
                return NotFound($"QuizAnswer con ID: {id} no fue encontrada");
            }

            await _unitOfWork.QuizAnswers.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
