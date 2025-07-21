using InteractiveAtlas.Infrastucture.Data;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly InteractiveAtlasDbContext _context;
        private readonly ProvinceRepository _provinceRepository;
        private readonly TypicalProductsRepository _typicalProductsRepository;
        private readonly TouristAttractionRepository _touristAttractionRepository;
        private readonly QuizQuestionRepository _quizQuestionRepository;
        private readonly QuizAnswerRepository _quizAnswerRepository;

        public UnitOfWork(InteractiveAtlasDbContext context, ProvinceRepository provinceRepository,
            TypicalProductsRepository typicalProductsRepository, TouristAttractionRepository touristAttractionRepository,
            QuizQuestionRepository quizQuestionRepository, QuizAnswerRepository quizAnswerRepository)
        {
            _context = context;
            _provinceRepository = provinceRepository;
            _typicalProductsRepository = typicalProductsRepository;
            _touristAttractionRepository = touristAttractionRepository;
            _quizQuestionRepository = quizQuestionRepository;
            _quizAnswerRepository = quizAnswerRepository;
        }

        // Forma con mas sentido y simplificada

        public InteractiveAtlasDbContext Context => _context;
        public ProvinceRepository Province =>_provinceRepository;
        public TypicalProductsRepository TypicalProducts => _typicalProductsRepository;
        public TouristAttractionRepository TouristAttraction => _touristAttractionRepository;
        public QuizQuestionRepository QuizQuestion => _quizQuestionRepository;
        public QuizAnswerRepository QuizAnswer => _quizAnswerRepository;




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
