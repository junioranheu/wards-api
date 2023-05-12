using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.Entities
{
    [Index(nameof(Email))]
    [Index(nameof(NomeUsuarioSistema))]
    public sealed class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;

        public string NomeUsuarioSistema { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string Chamado { get; set; } = string.Empty;

        public string Foto { get; set; } = string.Empty;

        public bool IsVerificado { get; set; } = false;

        public string? CodigoVerificacao { get; set; } = string.Empty;

        public DateTime? ValidadeCodigoVerificacao { get; set; }

        public string? HashUrlTrocarSenha { get; set; } = null;

        public DateTime? ValidadeHashUrlTrocarSenha { get; set; }

        public string? HistPerfisAtivos { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public bool IsLatest { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();

        public IEnumerable<UsuarioRole>? UsuarioRoles { get; init; }
    }
}