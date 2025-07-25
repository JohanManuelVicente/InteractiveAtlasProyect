using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Infrastucture.Data;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly InteractiveAtlasDbContext _context;
        
        //private readonly IProvinceRepository _provinceRepository;
      

        public IProvinceRepository Provinces { get; }
        public ITypicalProductRepository TypicalProducts { get; }
        public ITouristAttractionRepository TouristAttractions  { get; }
        public IQuizQuestionRepository QuizQuestions  { get; }
        public IQuizAnswerRepository QuizAnswers { get; }

        public UnitOfWork(InteractiveAtlasDbContext context, IProvinceRepository provinceRepository,
            ITypicalProductRepository typicalProductRepository, ITouristAttractionRepository touristAttractionRepository,
            IQuizQuestionRepository quizQuestionRepository, IQuizAnswerRepository quizAnswerRepository)
        {
            _context = context;
            Provinces = provinceRepository;
            TypicalProducts = typicalProductRepository;
            TouristAttractions = touristAttractionRepository;
            QuizQuestions = quizQuestionRepository;
            QuizAnswers = quizAnswerRepository;
           // _provinceRepository = provinceRepository;

        }

        // Forma con mas sentido y simplificada

        public InteractiveAtlasDbContext Context => _context;
        //public IProvinceRepository Province => _provinceRepository;
        //public ITypicalProductRepository TypicalProduct => _typicalProductRepository;
        //public ITouristAttractionRepository TouristAttraction => _touristAttractionRepository;
        //public IQuizQuestionRepository QuizQuestion => _quizQuestionRepository;
        //public IQuizAnswerRepository QuizAnswer => _quizAnswerRepository;




        // Formas sin mucho sentido pero muy comun en el mercado
        //public ProvinceRepository ProvinceRepository => _provinceRepository?? new ProvinceRepository(_context); 
        //public ProvinceRepository ProvinceRepository => _provinceRepository ?? throw new ArgumentNullException();

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task RollBackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
