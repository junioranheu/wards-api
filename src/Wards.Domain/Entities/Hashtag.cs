using System.ComponentModel.DataAnnotations;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class Hashtag
    {
        [Key]
        public int HashtagId { get; set; }

        public string Tag { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}