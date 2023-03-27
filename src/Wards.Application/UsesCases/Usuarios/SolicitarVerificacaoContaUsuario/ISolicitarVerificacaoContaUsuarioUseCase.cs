using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario
{
    public interface ISolicitarVerificacaoContaUsuarioUseCase
    {
        Task<UsuarioOutput?> Execute(int usuarioId);
    }
}