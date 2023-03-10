namespace Wards.Application.UsesCases.Logs.Shared.Input
{
    public sealed class LogInput
    {
        public string? TipoRequisicao { get; set; }

        public string? Endpoint { get; set; }

        public string? Parametros { get; set; }

        public int StatusResposta { get; set; }

        public int? UsuarioId { get; set; }
    }
}
