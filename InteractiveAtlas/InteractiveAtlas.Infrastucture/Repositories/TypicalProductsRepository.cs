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
    public class TypicalProductRepository : GenericRepository<TypicalProduct>, ITypicalProductRepository
    {
        private readonly InteractiveAtlasDbContext _context;

        public TypicalProductRepository(InteractiveAtlasDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<TypicalProduct>> GetAllTypicalProductByProvinceIdAsync(int provinceId)
        {
            return await _context.TypicalProducts.Where(t => t.ProvinceId == provinceId).ToListAsync();
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


    }
}
