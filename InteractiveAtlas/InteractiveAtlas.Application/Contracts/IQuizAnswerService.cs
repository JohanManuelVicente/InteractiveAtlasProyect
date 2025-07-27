using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Application.Contracts
{
    public interface IQuizAnswerService
    {
        Task<int> CreateQuizAnswer(QuizAnswerDto request);
        Task DeleteQuizAnswer(int id);
        Task<QuizAnswerDto> GetQuizAnswerById(int id);
        Task<List<QuizAnswerDto>> GetQuizAnswers();
        Task<List<QuizAnswerDto>> GetQuizAnswersByQuestionId(int questionId);
        Task<List<QuizAnswerDto>> GetQuizAnswersWithQuestion();
        Task UpdateQuizAnswer(int id, QuizAnswerDto request);
    }
}