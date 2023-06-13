using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class Log
    {
        [Key]
        public int LogId { get; set; }

        public string? TipoRequisicao { get; set; } = string.Empty;

        public string? Endpoint { get; set; } = string.Empty;

        public string? Parametros { get; set; } = string.Empty;

        public string? Descricao { get; set; } = string.Empty;

        public int StatusResposta { get; set; }

        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}
