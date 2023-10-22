using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Wards.Application.Hubs.Shared.Models.Output;
using Wards.Application.Hubs.Shared.Utils;

namespace Wards.Application.Hubs.ChatHub
{
    public sealed class ChatHub : Hub
    {
        const string usuario = "usuario";
        const string usuarioId = "usuarioId";
        const string grupo = "_online";
        private static readonly List<string> listaControladaDeUsuariosOnline = new();

        public override async Task OnConnectedAsync()
        {
            HttpRequest? request = (Context.GetHttpContext()?.Request) ?? throw new ArgumentNullException($"Houve um erro ao estabelecer conexão com o {nameof(ChatHub)} do SignalR.");

            if (!Misc.IsObjetoValido(request?.Query[usuario]) || !Misc.IsObjetoValido(request?.Query[usuarioId]))
            {
                throw new Exception($"O parâmetro de usuário é inválido");
            }

            // Salvar no contexto do Hub;
            Context.Items[usuario] = request?.Query[usuario];
            Context.Items[usuarioId] = request?.Query[usuarioId];

            // Adicionar o usuário no grupo (IGroupManager, nativo do SignalR);
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

            // Adicionar o usuário na lista de controle manual (diferentemente da lista nativa do SignalR acima; esse caso é para exibir os usuários on-line posteriormente);
            if (!listaControladaDeUsuariosOnline.Contains(Misc.ConverterObjetoParaString(Context.Items[usuarioId])))
            {
                listaControladaDeUsuariosOnline.Add(Misc.ConverterObjetoParaString(Context.Items[usuarioId]));
            }

            await EnviarMensagem(mensagem: $"O usuário {Context.Items[usuario]} entrou no chat", isAvisoSistema: true);
            await ObterListaUsuariosOnline();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (!string.IsNullOrEmpty(Misc.ConverterObjetoParaString(Context.Items[usuarioId])))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);
                listaControladaDeUsuariosOnline.Remove(Misc.ConverterObjetoParaString(Context.Items[usuarioId]));
                await EnviarMensagem(mensagem: $"O usuário {Context.Items[usuario]} saiu do chat", isAvisoSistema: true);
            }

            await ObterListaUsuariosOnline();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task EnviarMensagem(string mensagem, bool? isAvisoSistema = false)
        {
            ChatHubResponse data = new()
            {
                Mensagem = mensagem,
                UsuarioNome = isAvisoSistema.GetValueOrDefault() ? string.Empty : Misc.ConverterObjetoParaString(Context.Items[usuario]),
                UsuarioId = isAvisoSistema.GetValueOrDefault() ? string.Empty : Misc.ConverterObjetoParaString(Context.Items[usuarioId]),
                IsSistema = isAvisoSistema.GetValueOrDefault()
            };

            await Clients.Group(grupo).SendAsync("EnviarMensagem", data);
        }

        public async Task EnviarMensagemPrivada(string toUserId, string mensagem)
        {
            await Clients.User(toUserId).SendAsync("EnviarMensagemPrivada", Context.Items[usuario], mensagem);
        }

        public async Task ObterListaUsuariosOnline()
        {
            await Clients.Group(grupo).SendAsync("ObterListaUsuariosOnline", listaControladaDeUsuariosOnline);
        }
    }
}