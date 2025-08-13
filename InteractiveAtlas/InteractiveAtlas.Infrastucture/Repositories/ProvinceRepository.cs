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
    public class ProvinceRepository : GenericRepository<Province>, IProvinceRepository
    {
        private readonly InteractiveAtlasDbContext _context;

        public ProvinceRepository(InteractiveAtlasDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<Province>> GetAllProvincesWithDetailsAsync()
        {
            return await _context.Provinces
                .Include(p => p.TypicalProducts)
                .Include(p => p.TouristAttractions)
                .ToListAsync();
        }


    }
}
