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

    public class LogKey_TipoRequisicao_Data
    {
        public string? TipoRequisicao { get; set; }
        public DateTime Data { get; set; }

        public LogKey_TipoRequisicao_Data(string? tipoRequisicao, DateTime data)
        {
            TipoRequisicao = tipoRequisicao;
            Data = data;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LogKey_TipoRequisicao_Data x)
            {
                return TipoRequisicao == x.TipoRequisicao && (Data.Date == x.Data.Date && Data.Hour == x.Data.Hour);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TipoRequisicao, Data.Date, Data.Hour);
        }
    }

    public class LogKey_TipoRequisicao_StatusResposta
    {
        public string? TipoRequisicao { get; set; }
        public int StatusResposta { get; set; }

        public LogKey_TipoRequisicao_StatusResposta(string? tipoRequisicao, int statusResposta)
        {
            TipoRequisicao = tipoRequisicao;
            StatusResposta = statusResposta;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LogKey_TipoRequisicao_StatusResposta x)
            {
                return TipoRequisicao == x.TipoRequisicao && StatusResposta == x.StatusResposta;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TipoRequisicao, StatusResposta);
        }
    }
}