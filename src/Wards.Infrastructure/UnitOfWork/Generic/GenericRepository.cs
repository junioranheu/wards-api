using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wards.Infrastructure.Data;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly WardsContext _context;

        public GenericRepository(WardsContext context)
        {
            _context = context;
        }

        public async Task<List<T>> Listar(bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> Obter(Expression<Func<T, bool>> predicate, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<List<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrWhiteSpace(includeString))
            {
                query = query.Include(includeString);
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (includes is not null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T?> ObterComId(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> Criar(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> Atualizar(T entity)
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task Deletar(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}