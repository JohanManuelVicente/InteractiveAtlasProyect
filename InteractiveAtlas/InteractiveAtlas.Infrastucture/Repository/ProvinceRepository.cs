using InteractiveAtlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveAtlas.Infrastucture.Repository
{
    public class ProvinceRepository
    {
        private readonly InteractiveAtlasDbContext _context;

        public ProvinceRepository(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        public async Task<List<Province>> GetAllProvincesAsync()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<List<Province>> GetAllProvincesWithTypicalProductsAsync()
        {
            return await _context.Provinces
                .Include(p => p.TypicalProducts)
                .ToListAsync();
        }

        public async Task<Province?> GetProvinceByIdAsync(int id)
        {
            return await _context.Provinces.FindAsync();
        }

        public async Task<Province> AddProvinceAsync(Province province)
        {
            _context.Provinces.Add(province);
            await _context.SaveChangesAsync();
            return province;
        }

        public async Task<Province> UpdateProvinceAsync( Province province)
        {
            _context.Entry(province).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return province;
        }

        public async Task<bool> DeleteProvinceAsync (int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
            {
                return false;
            }

            _context.Provinces.Remove(province);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
