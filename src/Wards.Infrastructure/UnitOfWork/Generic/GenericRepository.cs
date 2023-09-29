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

        public async Task<List<T>> Listar(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include is not null)
            {
                query = include.Aggregate(query, (current, inc) => current.Include(inc));
            }

            if (where is not null)
            {
                query = query.Where(where);
            }

            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T?> Obter(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<T?> Obter(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include is not null)
            {
                query = include.Aggregate(query, (current, inc) => current.Include(inc));
            }

            if (where is not null)
            {
                query = query.Where(where);
            }

            return await query.FirstOrDefaultAsync();
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