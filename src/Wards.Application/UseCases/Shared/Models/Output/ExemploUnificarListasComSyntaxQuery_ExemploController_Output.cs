using Wards.Application.UseCases.Logs.Shared.Output;

namespace Wards.Application.UseCases.Shared.Models.Output
{
    public sealed class ExemploUnificarListasComSyntaxQuery_ExemploController_Output
    {
        public LogOutput Log { get; set; } = new();

        public string? Endpointjoin { get; set; } = string.Empty;

        public double? Exemplo1 { get; set; } = 0;

        public string? Exemplo2 { get; set; } = string.Empty;

        public bool? Exemplo3 { get; set; }
    }
}