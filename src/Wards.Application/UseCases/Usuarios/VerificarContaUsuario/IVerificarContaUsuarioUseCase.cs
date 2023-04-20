using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.VerificarContaUsuario
{
    public interface IVerificarContaUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(string codigoVerificacao);
    }
}