namespace Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands
{
    public interface ISolicitarVerificacaoContaUsuarioCommand
    {
        Task<(string, string, string, string)> Execute(int usuarioId);
    }
}