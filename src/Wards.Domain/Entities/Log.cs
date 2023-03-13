using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Log
    {
        [Key]
        public int LogId { get; set; }

        public string? TipoRequisicao { get; set; }

        public string? Endpoint { get; set; }

        public string? Parametros { get; set; }

        public int StatusResposta { get; set; }

        public int? UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
