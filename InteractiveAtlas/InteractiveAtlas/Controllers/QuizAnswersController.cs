using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Infrastucture.Repositories;
using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.Contracts;
using InteractiveAtlas.Services;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizAnswersController : ControllerBase
    {
        private readonly IQuizAnswerService _quizAnswerService;

        public QuizAnswersController(IQuizAnswerService quizAnswerService)
        {
            _quizAnswerService = quizAnswerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizAnswers()
        {
            return Ok(await _quizAnswerService.GetQuizAnswers());
        }

        [HttpGet]
        [Route("with-question")]
        public async Task<IActionResult> GetQuizAnswersWithQuestion()
        {
            return Ok(await _quizAnswerService.GetQuizAnswersWithQuestion());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizAnswerById(int id)
        {
         
            return Ok(await _quizAnswerService.GetQuizAnswerById(id));
        }

        [HttpGet]
        [Route("by-question")]
        public async Task<IActionResult> GetQuizAnswersByQuestionId([FromQuery] int questionId)
        {
            return Ok(await _quizAnswerService.GetQuizAnswersByQuestionId(questionId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuizAnswer([FromBody] QuizAnswerDto request)
        {
            var responseId = await _quizAnswerService.CreateQuizAnswer(request);
            return Ok(new { id = responseId});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizAnswer(int id, [FromBody] QuizAnswerDto request)
        {
            await _quizAnswerService.UpdateQuizAnswer(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAnswer(int id)
        {
            await _quizAnswerService.DeleteQuizAnswer(id);
            return NoContent();
        }
    }
}
