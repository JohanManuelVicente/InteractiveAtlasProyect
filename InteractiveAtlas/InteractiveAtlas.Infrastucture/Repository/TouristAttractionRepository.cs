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
    public class TouristAttractionRepository
    {
        private readonly InteractiveAtlasDbContext _context;
        public TouristAttractionRepository(InteractiveAtlasDbContext context)
        {
            _context = context;
        }
        public async Task<List<TouristAttraction>> GetAllTouristAttractionAsync()
        {
            return await _context.TouristAttractions.ToListAsync();
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
        public async Task<TouristAttraction?> GetTouristAttractionByIdAsync(int id)
        {
            return await _context.TouristAttractions
        .Include(t => t.Province) 
        .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<TouristAttraction> AddTouristAttractionAsync(TouristAttraction touristattraction)
        {
            _context.TouristAttractions.Add(touristattraction);
            await _context.SaveChangesAsync();
            return touristattraction;
        }
        public async Task<TouristAttraction> UpdateTouristAttractionAsync(TouristAttraction touristattraction)
        {
            _context.Entry(touristattraction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return touristattraction;
        }
        public async Task<bool> DeleteTouristAttractionAsync(int id)
        {
            var touristattraction = await _context.TouristAttractions.FindAsync(id);
            if (touristattraction == null)
            {
                return false;
            }
            _context.TouristAttractions.Remove(touristattraction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
