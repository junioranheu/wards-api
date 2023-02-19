using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Auths.Logar
{
    public interface ILogarUseCase
    {
        Task<UsuarioDTO> Logar(Usuario input);
    }
}