using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    public sealed class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? NomeUsuarioSistema { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Senha { get; set; } = string.Empty;

        public string? Chamado { get; set; } = string.Empty;

        public string? HistPerfisAtivos { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public bool IsLatest { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();

        public IEnumerable<UsuarioRole>? UsuarioRoles { get; init; }
    }
}