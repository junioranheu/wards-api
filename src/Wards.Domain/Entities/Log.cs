namespace Wards.Domain.Entities
{
    public sealed class Log
    {
        public int LogId { get; set; }

        public string? TipoRequisicao { get; set; }

        public string? Endpoint { get; set; }

        public string? Parametros { get; set; }

        public int StatusResposta { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = DateTime.UtcNow;
    }
}
