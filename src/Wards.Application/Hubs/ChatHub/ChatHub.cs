using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Wards.Application.Hubs.ChatHub
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        public async Task EnviarMensagem(string mensagem)
        {
            string usuario = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            await Clients.All.SendAsync("AEA_PE", usuario, mensagem);
        }

        public async Task EnviarMensagemPrivada(string toUserId, string mensagem)
        {
            string usuario = Context.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            await Clients.User(toUserId).SendAsync("AEA_PE_2", usuario, mensagem);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }
    }
}