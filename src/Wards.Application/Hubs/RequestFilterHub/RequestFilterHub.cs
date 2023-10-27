using Microsoft.AspNetCore.SignalR;
using Wards.Application.UseCases.Logs.Shared.Input;

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

        public async Task ExibirNovoRequest(LogInput log)
        {
            await Clients.All.SendAsync("ExibirNovoRequest", log);
        }
    }
}