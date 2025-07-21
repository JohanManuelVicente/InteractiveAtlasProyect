using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class TypicalProductRepository : GenericRepository<TypicalProduct>
    {
        private readonly InteractiveAtlasDbContext _context;

        public TypicalProductRepository(InteractiveAtlasDbContext context) : base (context)
        {
            _context = context;
        }

        //public async Task<List<TypicalProduct>> GetAllTypicalProductAsync()
        //{
        //    return await _context.TypicalProducts.ToListAsync();
        //}

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
        public async Task<TypicalProduct?> GetTypicalProductWithProvinceByIdAsync(int id)
        {
            return await _context.TypicalProducts
        .Include(t => t.Province)
        .FirstOrDefaultAsync(t => t.Id == id);
        }

        //public async Task<TypicalProduct> AddTypicalProductAsync(TypicalProduct typicalproduct)
        //{
        //    _context.TypicalProducts.Add(typicalproduct);
        //    return typicalproduct;
        //}

        //public async Task<TypicalProduct> UpdateTypicalProductAsync(TypicalProduct typicalproduct)
        //{
        //    _context.TypicalProducts.Update(typicalproduct);
        //    return typicalproduct;
        //}

        //public async Task<bool> DeleteTypicalProductAsync(int id)
        //{
        //    var typicalproduct = await _context.TypicalProducts.FindAsync(id);
        //    if (typicalproduct == null)
        //    {
        //        return false;
        //    }

        //    _context.TypicalProducts.Remove(typicalproduct);
        //    return true;
        //}
    }
}
