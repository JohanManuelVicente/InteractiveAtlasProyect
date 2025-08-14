﻿
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Infrastucture.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InteractiveAtlas.Infrastucture.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly InteractiveAtlasDbContext _context;

        public GenericRepository(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            return true;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? await _context.Set<T>().CountAsync()
                : await _context.Set<T>().CountAsync(predicate);

        }



    }
}
