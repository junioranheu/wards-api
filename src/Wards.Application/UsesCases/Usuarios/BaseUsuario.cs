using Wards.Domain.Enums;
using Wards.Utils.Entities;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios
{
    public class BaseUsuario
    {
        internal static async Task<bool> EnviarEmailVerificacaoConta(string emailTo, string nomeUsuario, string codigoVerificacao)
        {
            try
            {
                const string assunto = "Verifique sua conta";
                string nomeArquivo = GetDescricaoEnum(EmailEnum.VerificarConta);

                List<EmailDadosReplace> listaDadosReplace = new()
                {
                    new EmailDadosReplace { Key = "Url", Value = $"{CaminhoFront()}/usuario/verificar-conta/{codigoVerificacao}"},
                    new EmailDadosReplace { Key = "NomeUsuario", Value = nomeUsuario }
                };

                bool resposta = await EnviarEmail(emailTo, assunto, nomeArquivo, listaDadosReplace);
                return resposta;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}