using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.DTOs;
using InteractiveAtlas.Infrastucture.Repositories;
using InteractiveAtlas.Infrastucture.Data;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizQuestionsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public QuizQuestionsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizQuestions()
        {
            return Ok(await _unitOfWork.QuizQuestion.GetAllAsync());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetQuizQuestionsWithProvince()
        {
            var quizQuestions = await _unitOfWork.QuizQuestion.GetAllQuizQuestionWithProvinceAsync();

            var quizQuestionsResponse = new List<QuizQuestionDto>();
            quizQuestionsResponse = quizQuestions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                DifficultyLevel = q.DifficultyLevel,
                ProvinceId = q.ProvinceId,
                ProvinceName = q.Province?.Name
            }).ToList();
            return Ok(quizQuestionsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizQuestionById(int id)
        {
            var quizQuestion = await _unitOfWork.QuizQuestion.GetQuizQuestionWithProvinceByIdAsync(id);

            if (quizQuestion == null)
            {
                return BadRequest($"Quiz Question with ID: {id} not found");
            }

            var quizQuestionResponse = new QuizQuestionDto
            {
                Id = quizQuestion.Id,
                Text = quizQuestion.Text,
                DifficultyLevel = quizQuestion.DifficultyLevel,
                ProvinceId = quizQuestion.ProvinceId
            };
            return Ok(quizQuestionResponse);
        }

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GetQuizQuestionsByProvinceId([FromQuery] int provinceId)
        {
            return Ok(await _unitOfWork.QuizQuestion.GetAllQuizQuestionByProvinceIdAsync(provinceId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuizQuestion([FromBody] QuizQuestionDto request)
        {
            if (request == null)
            {
                return BadRequest("The QuizQuestion cannot be null");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
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

            quizQuestion = await _unitOfWork.QuizQuestion.AddAsync(quizQuestion);
            await _unitOfWork.CompleteAsync();
            return Ok(new { id = quizQuestion.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizQuestion(int id, [FromBody] QuizQuestionDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                return BadRequest("El texto de la pregunta es nulo");
            }

            var existingQuizQuestion = await _unitOfWork.QuizQuestion.GetQuizQuestionWithProvinceByIdAsync(id);
            if (existingQuizQuestion == null)
            {
                return NotFound($"La pregunta con ID {request.Id} no fue encontrada");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
                }
            }

            existingQuizQuestion.Text = request.Text;
            existingQuizQuestion.DifficultyLevel = request.DifficultyLevel;
            existingQuizQuestion.ProvinceId = request.ProvinceId;

            _unitOfWork.QuizQuestion.UpdateAsync(existingQuizQuestion).Wait();
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizQuestion(int id)
        {
            var quizQuestion = await _unitOfWork.QuizQuestion.GetByIdAsync(id);
            if (quizQuestion == null)
            {
                return NotFound($"QuizQuestion con ID: {id} no fue encontrada");
            }

            await _unitOfWork.QuizQuestion.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
