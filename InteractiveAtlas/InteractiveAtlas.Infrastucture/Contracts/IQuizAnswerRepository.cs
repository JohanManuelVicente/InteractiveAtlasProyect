using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface IQuizAnswerRepository : IRepository<QuizAnswer>
    {
        Task<List<QuizAnswer>> GetAllQuizAnswerByQuestionIdAsync(int questionId);
        Task<List<QuizAnswer>> GetAllQuizAnswerWithQuestionAsync();
        Task<QuizAnswer?> GetQuizAnswerWithQuizQuestionByIdAsync(int id);
    }
}