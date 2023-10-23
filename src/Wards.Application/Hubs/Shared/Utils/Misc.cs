using System.Security.Claims;
using Wards.Application.Hubs.Shared.Models.Output;

namespace Wards.Application.Hubs.Shared.Utils
{
    internal sealed class Misc
    {
        internal static bool IsObjetoValido(object? item)
        {
            try
            {
                if (string.IsNullOrEmpty(item?.ToString()) || item?.ToString() == "undefined")
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static string ConverterObjetoParaString(object? item)
        {
            try
            {
                return item?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static ChatHubResponse MontarChatHubResponse(string connectionId, List<UsuarioOnlineResponse> listaUsuarioOnline, ClaimsPrincipal? claims, string mensagem, bool? isAvisoSistema = false, string? usuarioIdDestinatario = null)
        {
            if (!isAvisoSistema.GetValueOrDefault() && !listaUsuarioOnline.Any(x => x.ConnectionId == connectionId))
            {
                throw new Exception($"ConnectionId inválido"); // Quando por exemplo, um usuário entra em uma nova aba, ele inválida a sessão (ConnectionId);
            }

            string usuarioNome = ConverterObjetoParaString(claims?.FindFirst(ClaimTypes.Name)?.Value);
            string usuarioId = ConverterObjetoParaString(claims?.FindFirst(ClaimTypes.Email)?.Value);

            ChatHubResponse response = new()
            {
                Mensagem = mensagem,
                UsuarioNome = isAvisoSistema.GetValueOrDefault() ? null : usuarioNome,
                UsuarioId = isAvisoSistema.GetValueOrDefault() ? null : usuarioId,
                IsSistema = isAvisoSistema.GetValueOrDefault(),
                UsuarioIdDestinatario = usuarioIdDestinatario
            };

            return response;
        }
    }
}