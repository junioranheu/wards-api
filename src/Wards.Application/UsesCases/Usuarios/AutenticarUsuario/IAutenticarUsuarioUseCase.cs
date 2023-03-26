using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.AutenticarUsuario
{
    public interface IAutenticarUsuarioUseCase
    {
        Task<AutenticarUsuarioOutput> Execute(AutenticarUsuarioInput input);
    }
}