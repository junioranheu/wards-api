using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Auths.Registrar
{
    public interface IRegistrarUseCase
    {
        Task<(UsuarioOutput?, string)> Execute(RegistrarInput input);
    }
}