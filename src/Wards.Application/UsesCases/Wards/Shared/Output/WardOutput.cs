using Wards.Application.UsesCases.Usuarios.Shared.Output;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.Shared.Output
{
    public sealed class WardOutput
    {
        public int WardId { get; set; }

        public string? Conteudo { get; set; }

        public int? UsuarioId { get; set; }
        public UsuarioOutput? Usuarios { get; init; }

        public DateTime Data { get; set; } = HorarioBrasilia();

        public bool IsAtivo { get; set; } = true;
    }
}
