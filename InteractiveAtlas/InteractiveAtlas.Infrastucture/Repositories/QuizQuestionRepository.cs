using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class QuizQuestionRepository : GenericRepository<QuizQuestion>, IQuizQuestionRepository
    {
        private readonly InteractiveAtlasDbContext _context;
        public QuizQuestionRepository(InteractiveAtlasDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<QuizQuestion>> GetAllQuizQuestionByProvinceIdAsync(int provinceId)
        {
            return await _context.QuizQuestions
               .Include(q => q.Province)
               .Where(q => q.ProvinceId == provinceId)
               .ToListAsync();
        }
        public async Task<List<QuizQuestion>> GetAllQuizQuestionWithProvinceAsync()
        {
            return await _context.QuizQuestions
                .Include(p => p.Province)
                .ToListAsync();
        }
        public async Task<QuizQuestion?> GetQuizQuestionWithProvinceByIdAsync(int id)
        {
            return await _context.QuizQuestions
                 .Include(q => q.Province)
                 .FirstOrDefaultAsync(q => q.Id == id);
        }

    }
}
