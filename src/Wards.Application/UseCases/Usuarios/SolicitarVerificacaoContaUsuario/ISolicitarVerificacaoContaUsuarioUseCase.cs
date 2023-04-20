using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario
{
    public interface ISolicitarVerificacaoContaUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(int usuarioId);
    }
}