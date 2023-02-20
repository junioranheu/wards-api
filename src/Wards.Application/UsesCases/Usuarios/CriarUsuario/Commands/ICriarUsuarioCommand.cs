using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands
{
    public interface ICriarUsuarioCommand
    {
        Task<UsuarioDTO> Criar(Usuario input);
    }
}