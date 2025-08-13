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
    public class TouristAttractionRepository : GenericRepository<TouristAttraction>, ITouristAttractionRepository
    {
        private readonly InteractiveAtlasDbContext _context;
        public TouristAttractionRepository(InteractiveAtlasDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TouristAttraction>> GetAllTouristAttractionByProvinceIdAsync(int provinceId)
        {
            return await _context.TouristAttractions.Where(t => t.ProvinceId == provinceId).ToListAsync();
        }
        public async Task<List<TouristAttraction>> GetAllTouristAttractionWithProvinceAsync()
        {
            return await _context.TouristAttractions
                .Include(p => p.Province)
                .ToListAsync();
        }
        public async Task<TouristAttraction?> GetTouristAttractionWithProvinceByIdAsync(int id)
        {
            return await _context.TouristAttractions
        .Include(t => t.Province)
        .FirstOrDefaultAsync(t => t.Id == id);
        }

    }
}
