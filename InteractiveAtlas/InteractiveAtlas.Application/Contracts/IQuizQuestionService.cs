using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Application.Contracts
{
    public interface IQuizQuestionService
    {
        Task<int> CreateQuizQuestion(QuizQuestionDto request);
        Task DeleteQuizQuestion(int id);
        Task<QuizQuestionDto> GetQuizQuestionById(int id);
        Task<List<QuizQuestionDto>> GetQuizQuestions();
        Task<List<QuizQuestionDto>> GetQuizQuestionsByProvinceId(int provinceId);
        Task<List<QuizQuestionDto>> GetQuizQuestionsWithProvince();
        Task UpdateQuizQuestion(int id, QuizQuestionDto request);
    }
}