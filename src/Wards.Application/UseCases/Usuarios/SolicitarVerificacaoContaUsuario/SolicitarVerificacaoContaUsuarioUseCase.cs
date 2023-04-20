using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands;
using Wards.Domain.Enums;
using static Wards.Utils.Common;

namespace Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario
{
    public sealed class SolicitarVerificacaoContaUsuarioUseCase : BaseUsuario, ISolicitarVerificacaoContaUsuarioUseCase
    {
        private readonly ISolicitarVerificacaoContaUsuarioCommand _solicitarVerificacaoContaUsuarioCommand;

        public SolicitarVerificacaoContaUsuarioUseCase(ISolicitarVerificacaoContaUsuarioCommand solicitarVerificacaoContaUsuarioCommand)
        {
            _solicitarVerificacaoContaUsuarioCommand = solicitarVerificacaoContaUsuarioCommand;
        }

        public async Task<UsuarioOutput?> Execute(int usuarioId)
        {
            (string mensagemErro, string email, string nomeCompleto, string codigoVerificacao) resp = await _solicitarVerificacaoContaUsuarioCommand.Execute(usuarioId);
            string erros = resp.mensagemErro;

            if (!string.IsNullOrEmpty(erros))
            {
                return (new UsuarioOutput() { Messages = new string[] { erros } });
            }

            string email = resp.email;
            string nomeCompleto = resp.nomeCompleto;
            string codigoVerificacao = resp.codigoVerificacao;

            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(nomeCompleto) && !string.IsNullOrEmpty(codigoVerificacao))
                {
                    await EnviarEmailVerificacaoConta(email, nomeCompleto, codigoVerificacao);
                }
            }
            catch (Exception)
            {
                return (new UsuarioOutput() { Messages = new string[] { ObterDescricaoEnum(CodigoErroEnum.ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao) } });
            }

            return new UsuarioOutput();
        }
    }
}