using System.Linq.Expressions;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public interface IGenericRepository<T>
    {
        Task<List<T>> Listar();
        Task<List<T>> Obter(Expression<Func<T, bool>> predicate);
        Task<List<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null, bool disableTracking = true);
        Task<List<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true);
        Task<T?> ObterComId(int id);
        Task<T> Criar(T entity);
        Task<T> Atualizar(T entity);
        Task Deletar(T entity);
    }
}