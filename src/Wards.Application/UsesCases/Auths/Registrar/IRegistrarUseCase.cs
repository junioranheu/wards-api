using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Auths.Registrar
{
    public interface IRegistrarUseCase
    {
        Task<UsuarioDTO> Registrar(Usuario input);
    }
}