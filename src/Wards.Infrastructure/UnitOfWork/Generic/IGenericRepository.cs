using System.Linq.Expressions;

namespace Wards.Infrastructure.UnitOfWork.Generic
{
    public interface IGenericRepository<T>
    {
        Task<T> Atualizar(T entity);
        Task<T> Criar(T entity);
        Task Deletar(T entity);
        Task<List<T>> ListarTudo(bool disableTracking = true);
        Task<List<T>> Listar(Expression<Func<T, bool>>? where = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? include = null, bool disableTracking = true);
        Task<T?> ObterComId(int id);
    }
}