using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Repositories;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface IUnitOfWork
    {
        InteractiveAtlasDbContext Context { get; }
        IProvinceRepository Provinces { get; }
        IQuizAnswerRepository QuizAnswers { get; }
        IQuizQuestionRepository QuizQuestions { get; }
        ITouristAttractionRepository TouristAttractions { get; }
        ITypicalProductRepository TypicalProducts { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task CompleteAsync();
        void Dispose();
        Task RollBackTransactionAsync();
    }
}