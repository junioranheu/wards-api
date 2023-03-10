using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public interface IObterUsuarioQuery
    {
        Task<Usuario?> Execute(int id, string email);
    }
}