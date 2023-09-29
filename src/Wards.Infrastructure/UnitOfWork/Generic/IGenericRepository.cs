using System.Linq.Expressions;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public interface IGenericRepository<T>
    {
        Task<T> Atualizar(T entity);
        Task<T> Criar(T entity);
        Task Deletar(T entity);
        Task<List<T>> Listar(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true);
        Task<List<TResult>> Listar<TResult>(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true);
        Task<T?> Obter(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true);
        Task<T?> Obter(int id);
        Task<TResult?> Obter<TResult>(Expression<Func<T, bool>>? where = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true);
        Task<TResult?> Obter<TResult>(int id);
    }
}