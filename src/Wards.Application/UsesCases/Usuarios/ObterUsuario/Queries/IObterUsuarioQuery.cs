using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries
{
    public interface IObterUsuarioQuery
    {
        Task<UsuarioDTO> Obter(int id);
        Task<UsuarioDTO> ObterByEmail(string email);
        Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema);
    }
}