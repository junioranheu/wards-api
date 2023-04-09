namespace Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands
{
    public interface ISolicitarVerificacaoContaUsuarioCommand
    {
        Task<(string mensagemErro, string email, string nomeCompleto, string codigoVerificacao)> Execute(int usuarioId);
    }
}