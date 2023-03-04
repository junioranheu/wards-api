using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public interface IObterUsuarioQuery
    {
        Task<Usuario?> Obter(int id, string email);
        Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema);
    }
}