using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveAtlas.Infrastucture.Repository
{
    public class QuizQuestionRepository
    {
        private readonly InteractiveAtlasDbContext _context;
        public QuizQuestionRepository(InteractiveAtlasDbContext context)
        {
            _context = context;
        }
        public async Task<List<QuizQuestion>> GetAllQuizQuestionAsync()
        {
            return await _context.QuizQuestions.ToListAsync();
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
        public async Task<QuizQuestion?> GetQuizQuestionByIdAsync(int id)
        {
            return await _context.QuizQuestions
                 .Include(q => q.Province) 
                 .FirstOrDefaultAsync(q => q.Id == id);
        }
        public async Task<QuizQuestion> AddQuizQuestionAsync(QuizQuestion quizquestion)
        {
            _context.QuizQuestions.Add(quizquestion);
            await _context.SaveChangesAsync();
            return quizquestion;
        }
        public async Task<QuizQuestion> UpdateQuizQuestionAsync(QuizQuestion quizquestion)
        {
            _context.Entry(quizquestion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return quizquestion;
        }
        public async Task<bool> DeleteQuizQuestionAsync(int id)
        {
            var quizquestion = await _context.QuizQuestions.FindAsync(id);
            if (quizquestion == null)
            {
                return false;
            }
            _context.QuizQuestions.Remove(quizquestion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
