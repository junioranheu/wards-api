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

        private IQueryable<T> PrepararQuery(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
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

            return query;
        }

        public async Task<List<T>> Listar(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            var query = PrepararQuery(where, include, disableTracking);

            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<List<TResult>> Listar<TResult>(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            var query = PrepararQuery(where, include, disableTracking);

            if (orderBy is not null)
            {
                return _map.Map<List<TResult>>(await orderBy(query).ToListAsync());
            }

            return _map.Map<List<TResult>>(await query.ToListAsync());
        }

        public async Task<T?> Obter(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<TResult?> Obter<TResult>(int id)
        {
            return _map.Map<TResult>(await _context.Set<T>().FindAsync(id));
        }

        public async Task<T?> Obter(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            var query = PrepararQuery(where, include, disableTracking);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<TResult?> Obter<TResult>(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true)
        {
            var query = PrepararQuery(where, include, disableTracking);
            return _map.Map<TResult>(await query.FirstOrDefaultAsync());
        }

        public async Task<T> Criar(T entidade)
        {
            _context.Set<T>().Add(entidade);
            await _context.SaveChangesAsync();

            return entidade;
        }

        public async Task<T> Atualizar(T entidade)
        {
            _context.Set<T>().Attach(entidade);
            _context.Entry(entidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entidade;
        }

        public async Task Deletar(T entidade)
        {
            _context.Set<T>().Remove(entidade);
            await _context.SaveChangesAsync();
        }
    }
}