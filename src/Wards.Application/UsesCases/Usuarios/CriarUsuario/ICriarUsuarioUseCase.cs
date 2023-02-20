using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<UsuarioDTO> Criar(Usuario input);
    }
}