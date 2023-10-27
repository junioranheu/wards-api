using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.Hubs.ChatHub.Models.Output
{
    public sealed class UsuarioOnlineResponse
    {
        public string UsuarioNome { get; set; } = string.Empty;

        public string UsuarioId { get; set; } = string.Empty;

        public string ConnectionId { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = GerarHorarioBrasilia();
    }
}