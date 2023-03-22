using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Logs.Shared.Output
{
    public sealed class LogOutput : ApiResponse
    {
        public string? TipoRequisicao { get; set; }

        public string? Endpoint { get; set; }

        public string? Parametros { get; set; }

        public int StatusResposta { get; set; }

        public int? UsuarioId { get; set; }
        public UsuarioOutput? Usuarios { get; init; }

        public DateTime Data { get; set; }
    }
}