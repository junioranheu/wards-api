using System.Linq.Expressions;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public interface IGenericRepository<T>
    {
        Task<T> Atualizar(T entity);
        Task<T> Criar(T entity);
        Task Deletar(T entity);
        Task<IReadOnlyList<T>> Listar();
        Task<IReadOnlyList<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true);
        Task<IReadOnlyList<T>> Obter(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null, bool disableTracking = true);
        Task<IReadOnlyList<T>> Obter(Expression<Func<T, bool>> predicate);
        Task<T?> ObterComId(int id);
    }
}