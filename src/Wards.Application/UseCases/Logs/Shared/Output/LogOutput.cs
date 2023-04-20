using Wards.Application.UseCases.Shared.Models;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Logs.Shared.Output
{
    public sealed class LogOutput : ApiOutput
    {
        public int LogId { get; set; }

        public string? TipoRequisicao { get; set; }

        public string? Endpoint { get; set; }

        public string? Parametros { get; set; }

        public string? Descricao { get; set; }

        public int StatusResposta { get; set; }

        public int? UsuarioId { get; set; }
        public UsuarioOutput? Usuarios { get; init; }

        public DateTime Data { get; set; }
    }
}