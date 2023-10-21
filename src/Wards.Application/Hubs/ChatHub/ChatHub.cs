using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Wards.Application.Hubs.ChatHub
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        readonly string _usuario;

        public ChatHub()
        {
            _usuario = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public async Task EnviarMensagem(string mensagem)
        {
            await Clients.All.SendAsync("AEA_PE", _usuario, mensagem);
        }

        public async Task EnviarMensagemPrivada(string toUserId, string mensagem)
        {
            await Clients.User(toUserId).SendAsync("AEA_PE_2", _usuario, mensagem);
        }

        public override async Task OnConnectedAsync()
        {
            string? usuario = Context.User?.Identity?.Name ?? null;

            if (usuario is not null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, usuario);
            }

            await base.OnConnectedAsync();
        }
    }
}