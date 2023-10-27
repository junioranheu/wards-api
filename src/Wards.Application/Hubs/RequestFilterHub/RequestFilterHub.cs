using Microsoft.AspNetCore.SignalR;

namespace Wards.Application.Hubs.RequestFilterHub
{
    public sealed class RequestFilterHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task ExibirNovoRequest(string mensagem)
        {
            await Clients.All.SendAsync("ExibirNovoRequest", mensagem);
        }
    }
}