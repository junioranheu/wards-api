namespace Wards.Application.UseCases.Logs.Shared.Output
{
    public sealed class LogAgrupadoOutput
    {
        public DateTime Data { get; set; }

        public string? DataStr { get; set; } = string.Empty;

        public int? QtdLogs { get; set; }

        public int? UsuarioId { get; set; }

        public string? UsuarioNome { get; set; } = string.Empty;

        public int[]? ListaStatusResposta { get; set; }
    }
}