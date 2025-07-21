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
    public class TypicalProductsRepository
    {
        private readonly InteractiveAtlasDbContext _context;

        public TypicalProductsRepository(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        public async Task<List<TypicalProduct>> GetAllTypicalProductAsync()
        {
            return await _context.TypicalProducts.ToListAsync();
        }

        public async Task<List<TypicalProduct>> GetAllTypicalProductByProvinceIdAsync(int provinceId)
        {
            return await _context.TypicalProducts.Where(t=> t.ProvinceId == provinceId).ToListAsync();
        }

        public async Task<List<TypicalProduct>> GetAllTypicalProductWithProvinceAsync()
        {
            return await _context.TypicalProducts
                .Include(p => p.Province)
                .ToListAsync();
        }
        public async Task<TypicalProduct?> GetTypicalProductByIdAsync(int id)
        {
            return await _context.TypicalProducts.FindAsync(id);
        }

        public async Task<TypicalProduct> AddTypicalProductAsync(TypicalProduct typicalproduct)
        {
            _context.TypicalProducts.Add(typicalproduct);
            await _context.SaveChangesAsync();
            return typicalproduct;
        }

        public async Task<TypicalProduct> UpdateTypicalProductAsync(TypicalProduct typicalproduct)
        {
            _context.Entry(typicalproduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return typicalproduct;
        }

        public async Task<bool> DeleteTypicalProductAsync(int id)
        {
            var typicalproduct = await _context.TypicalProducts.FindAsync(id);
            if (typicalproduct == null)
            {
                return false;
            }

            _context.TypicalProducts.Remove(typicalproduct);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
