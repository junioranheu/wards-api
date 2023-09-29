using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wards.Infrastructure.Data;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly WardsContext _context;
        private readonly IMapper _map;

        public GenericRepository(WardsContext context, IMapper map)
        {
            _context = context;
            _map = map;
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

        public async Task<List<TResult>> Listar<TResult>(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
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
                var linqOrderBy = await orderBy(query).ToListAsync();
                var linqOrderByMapped = _map.Map<List<TResult>>(linqOrderBy);

                return linqOrderByMapped;
            }

            var linq = await query.ToListAsync();
            var linqMapped = _map.Map<List<TResult>>(linq);

            return linqMapped;
        }

        public virtual async Task<T?> Obter(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<TResult?> Obter<TResult>(int id)
        {
            var linq = await _context.Set<T>().FindAsync(id);
            var linqMapped = _map.Map<TResult>(linq);

            return linqMapped;
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

        public virtual async Task<TResult?> Obter<TResult>(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
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

            var linq = await query.FirstOrDefaultAsync();
            var linqMapped = _map.Map<TResult>(linq);

            return linqMapped;
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