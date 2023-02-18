using Wards.Domain.DTOs;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public interface ILogarUseCase
    {
        Task<UsuarioDTO> Logar(UsuarioDTO dto);
    }
}