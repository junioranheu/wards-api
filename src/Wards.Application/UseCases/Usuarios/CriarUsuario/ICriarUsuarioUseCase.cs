using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.CriarUsuario
{
    public interface ICriarUsuarioUseCase
    {
        Task<AutenticarUsuarioOutput?> Execute(CriarUsuarioInput input);
    }
}