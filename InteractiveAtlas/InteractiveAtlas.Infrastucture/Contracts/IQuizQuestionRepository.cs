using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface IQuizQuestionRepository : IRepository<QuizQuestion>
    {
        Task<List<QuizQuestion>> GetAllQuizQuestionByProvinceIdAsync(int provinceId);
        Task<List<QuizQuestion>> GetAllQuizQuestionWithProvinceAsync();
        Task<QuizQuestion?> GetQuizQuestionWithProvinceByIdAsync(int id);
    }
}