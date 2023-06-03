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
            (string mensagemErro, string email, string nomeCompleto, string codigoVerificacao) = await _solicitarVerificacaoContaUsuarioCommand.Execute(usuarioId);
            string erros = mensagemErro;

            if (!string.IsNullOrEmpty(erros))
            {
                throw new Exception(erros);
            }

            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(nomeCompleto) && !string.IsNullOrEmpty(codigoVerificacao))
                {
                    await EnviarEmailVerificacaoConta(email, nomeCompleto, codigoVerificacao);
                }
            }
            catch (Exception)
            {
                throw new Exception(ObterDescricaoEnum(CodigoErroEnum.ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao));
            }

            return new UsuarioOutput();
        }
    }
}