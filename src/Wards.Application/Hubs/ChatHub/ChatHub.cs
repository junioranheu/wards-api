using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Wards.Application.Hubs.Shared.Models.Output;

namespace Wards.Application.Hubs.ChatHub
{
    public sealed class ChatHub : Hub
    {
        const string usuario = "usuario";
        const string usuarioId = "usuarioId";

        public override async Task OnConnectedAsync()
        {
            HttpRequest? request = Context.GetHttpContext()?.Request;

            if (request is null)
            {
                throw new ArgumentNullException($"Houve um erro ao estabelecer conexão com o {nameof(ChatHub)} do SignalR.");
            }

            Context.Items[usuario] = request?.Query[usuario];
            Context.Items[usuarioId] = request?.Query[usuarioId];

            if (Context.Items[usuarioId] is not null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ConverterObjetoParaString(Context.Items[usuarioId]));
            }

            await base.OnConnectedAsync();
        }

        public async Task EnviarMensagem(string mensagem)
        {
            ChatHubResponse data = new()
            {
                Mensagem = mensagem,
                UsuarioNome = ConverterObjetoParaString(Context.Items[usuario]),
                UsuarioId = ConverterObjetoParaString(Context.Items[usuarioId])
            };

            await Clients.All.SendAsync("EnviarMensagem", data);
        }

        public async Task EnviarMensagemPrivada(string toUserId, string mensagem)
        {
            await Clients.User(toUserId).SendAsync("EnviarMensagemPrivada", Context.Items[usuario], mensagem);
        }

        private static string ConverterObjetoParaString(object? item)
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
    }
}