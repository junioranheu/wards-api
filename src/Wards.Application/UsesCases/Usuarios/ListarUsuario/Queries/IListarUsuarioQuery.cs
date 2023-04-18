using Wards.Application.UsesCases.Shared.Models;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries
{
    public interface IListarUsuarioQuery
    {
        Task<IEnumerable<Usuario>?> Execute(PaginacaoInput input);
    }
}