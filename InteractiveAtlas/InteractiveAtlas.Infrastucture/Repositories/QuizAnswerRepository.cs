using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class QuizAnswerRepository : GenericRepository<QuizAnswer>, IQuizAnswerRepository
    {
        private readonly InteractiveAtlasDbContext _context;
        public QuizAnswerRepository(InteractiveAtlasDbContext context) : base(context)
        {
            _context = context;
        }
        //public async Task<List<QuizAnswer>> GetAllQuizAnswerAsync()
        //{
        //    return await _context.QuizAnswers.ToListAsync();
        //}
        public async Task<List<QuizAnswer>> GetAllQuizAnswerByQuestionIdAsync(int questionId)
        {
            return await _context.QuizAnswers
            .Include(a => a.Question)
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
        }
        public async Task<List<QuizAnswer>> GetAllQuizAnswerWithQuestionAsync()
        {
            return await _context.QuizAnswers
                .Include(p => p.Question)
                .ToListAsync();
        }
        public async Task<QuizAnswer?> GetQuizAnswerWithQuizQuestionByIdAsync(int id)
        {
            return await _context.QuizAnswers
           .Include(a => a.Question)
           .FirstOrDefaultAsync(a => a.Id == id);

        }
        //public async Task<QuizAnswer> AddQuizAnswerAsync(QuizAnswer quizanswer)
        //{
        //    _context.QuizAnswers.Add(quizanswer);

        //    return quizanswer;
        //}
        //public async Task<QuizAnswer> UpdateQuizAnswerAsync(QuizAnswer quizanswer)
        //{
        //    _context.QuizAnswers.Update(quizanswer);

        //    return quizanswer;
        //}
        //public async Task<bool> DeleteQuizAnswerAsync(int id)
        //{
        //    var quizanswer = await _context.QuizAnswers.FindAsync(id);
        //    if (quizanswer == null)
        //    {
        //        return false;
        //    }
        //    _context.QuizAnswers.Remove(quizanswer);

        //    return true;
        //}
    }
}
