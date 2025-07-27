using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.Infrastucture.Repositories;
using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Application.Contracts;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizQuestionsController : ControllerBase
    {
        private readonly IQuizQuestionService _quizQuestionService;

        public QuizQuestionsController(IQuizQuestionService quizQuestionService)
        {
           _quizQuestionService = quizQuestionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizQuestions()
        {
            return Ok(await _quizQuestionService.GetQuizQuestions());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetQuizQuestionsWithProvince()
        {
            return Ok(await _quizQuestionService.GetQuizQuestionsWithProvince());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizQuestionById(int id)
        {
            return Ok(await _quizQuestionService.GetQuizQuestionById(id));
        }

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GetQuizQuestionsByProvinceId([FromQuery] int provinceId)
        {
            return Ok(await _quizQuestionService.GetQuizQuestionsByProvinceId(provinceId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuizQuestion([FromBody] QuizQuestionDto request)
        {
            var responseId = await _quizQuestionService.CreateQuizQuestion(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuizQuestion(int id, [FromBody] QuizQuestionDto request)
        {
            await _quizQuestionService.UpdateQuizQuestion(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizQuestion(int id)
        {
           await _quizQuestionService.DeleteQuizQuestion(id);
            return NoContent();
        }
    }
}
