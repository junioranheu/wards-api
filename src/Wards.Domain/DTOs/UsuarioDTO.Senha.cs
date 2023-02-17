using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Common;

namespace Wards.Domain.DTOs
{
    public class UsuarioSenhaDTO : _RetornoApiDTO
    {
        [Key]
        public int UsuarioId { get; set; }

        public string? NomeCompleto { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = HorarioBrasilia();
    }
}
