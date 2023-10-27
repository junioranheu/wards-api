using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.Hubs.ChatHub.Models.Output
{
    public sealed class ChatHubResponse
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Mensagem { get; set; } = string.Empty;

        public string? UsuarioNome { get; set; } = string.Empty;

        public string? UsuarioId { get; set; } = string.Empty;

        public string? UsuarioIdDestinatario { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = GerarHorarioBrasilia();

        public bool IsSistema { get; set; } = false;
    }
}