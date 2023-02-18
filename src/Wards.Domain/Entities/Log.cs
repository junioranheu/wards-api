using System.ComponentModel.DataAnnotations;
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

        public int UsuarioRoleId { get; set; }
        public UsuarioRole? UsuarioRoles { get; init; }

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
