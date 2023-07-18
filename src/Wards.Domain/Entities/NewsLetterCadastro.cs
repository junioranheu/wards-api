using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class NewsLetterCadastro
    {
        [Key]
        public int NewsLetterCadastroId { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}