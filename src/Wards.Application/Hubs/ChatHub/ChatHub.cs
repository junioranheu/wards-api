using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Wards.Application.Hubs.ChatHub.Models.Output;
using Wards.Application.Hubs.Shared.Utils;

namespace Wards.Application.Hubs.ChatHub
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        const string grupo = "_online";
        private static readonly List<UsuarioOnlineResponse> listaUsuarioOnline = new();

        public override async Task OnConnectedAsync()
        {
            if (!Context.User!.Identity!.IsAuthenticated)
            {
                throw new Exception($"Usuário não autenticado");
            }

            string usuarioNome = Misc.ConverterObjetoParaString(Context.User.FindFirst(ClaimTypes.Name)?.Value);
            string usuarioId = Misc.ConverterObjetoParaString(Context.User.FindFirst(ClaimTypes.Email)?.Value);
            string signalR_ConnectionId = Misc.ConverterObjetoParaString(Context.ConnectionId);

            // Adicionar o usuário (signalR_ConnectionId) no grupo (IGroupManager, nativo do SignalR);
            await Groups.AddToGroupAsync(signalR_ConnectionId, grupo);

            // Adicionar/atualizar usuário na lista de controle manual;
            UsuarioOnlineResponse? checkUsuarioOnline = listaUsuarioOnline.Where(x => x.UsuarioId == usuarioId).FirstOrDefault();

            if (checkUsuarioOnline is null)
            {
                UsuarioOnlineResponse u = new()
                {
                    UsuarioNome = usuarioNome,
                    UsuarioId = usuarioId,
                    ConnectionId = signalR_ConnectionId
                };

                listaUsuarioOnline.Add(u);
                await EnviarMensagem(mensagem: $"O usuário {usuarioNome} entrou no chat", isAvisoSistema: true);
            }
            else
            {
                await Groups.RemoveFromGroupAsync(checkUsuarioOnline.ConnectionId, grupo); // Remover ConnectionId antigo;
                checkUsuarioOnline.ConnectionId = signalR_ConnectionId;
            }

            await ObterListaUsuariosOnline();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string signalR_ConnectionId = Misc.ConverterObjetoParaString(Context.ConnectionId);
            UsuarioOnlineResponse? checkUsuario = listaUsuarioOnline.FirstOrDefault(x => x.ConnectionId == signalR_ConnectionId);

            if (checkUsuario is not null)
            {
                await Groups.RemoveFromGroupAsync(signalR_ConnectionId, grupo);

                await EnviarMensagem(mensagem: $"O usuário {checkUsuario?.UsuarioNome} saiu do chat", isAvisoSistema: true);
                listaUsuarioOnline.Remove(checkUsuario!);
            }

            await ObterListaUsuariosOnline();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task EnviarMensagem(string mensagem, bool? isAvisoSistema = false)
        {
            ChatHubResponse response = CriarResponse(Context.ConnectionId, listaUsuarioOnline, Context.User, mensagem, isAvisoSistema.GetValueOrDefault());
            await Clients.Group(grupo).SendAsync("EnviarMensagem", response);
        }

        public async Task EnviarMensagemPrivada(string usuarioIdDestinatario, string mensagem, bool? isAvisoSistema = false)
        {
            UsuarioOnlineResponse? checkUsuarioDestinatario = listaUsuarioOnline.FirstOrDefault(x => x.UsuarioId == usuarioIdDestinatario) ?? throw new Exception($"Usuário não encontrado");

            ChatHubResponse response = CriarResponse(Context.ConnectionId, listaUsuarioOnline, Context.User, mensagem, isAvisoSistema.GetValueOrDefault(), usuarioIdDestinatario);
            await Clients.Client(Context.ConnectionId).SendAsync("EnviarMensagemPrivada", response);
            await Clients.Client(checkUsuarioDestinatario?.ConnectionId!).SendAsync("EnviarMensagemPrivada", response);
        }

        public async Task ObterListaUsuariosOnline()
        {
            await Clients.Group(grupo).SendAsync("ObterListaUsuariosOnline", listaUsuarioOnline);
        }

        private static ChatHubResponse CriarResponse(string connectionId, List<UsuarioOnlineResponse> listaUsuarioOnline, ClaimsPrincipal? claims, string mensagem, bool? isAvisoSistema = false, string? usuarioIdDestinatario = null)
        {
            if (!isAvisoSistema.GetValueOrDefault() && !listaUsuarioOnline.Any(x => x.ConnectionId == connectionId))
            {
                throw new Exception($"ConnectionId inválido"); // Quando por exemplo, um usuário entra em uma nova aba, ele inválida a sessão (ConnectionId);
            }

            string usuarioNome = Misc.ConverterObjetoParaString(claims?.FindFirst(ClaimTypes.Name)?.Value);
            string usuarioId = Misc.ConverterObjetoParaString(claims?.FindFirst(ClaimTypes.Email)?.Value);

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