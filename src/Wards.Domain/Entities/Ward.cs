using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class Ward
    {
        [Key]
        public int WardId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public byte[]? ImagemPrincipalBlob { get; set; } = Array.Empty<byte>();

        public string Conteudo { get; set; } = string.Empty;

        public int QtdCurtidas { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuarios { get; init; }

        public DateTime Data { get; set; } = GerarHorarioBrasilia();

        public int? UsuarioModId { get; set; }
        [ForeignKey(nameof(UsuarioModId))]
        public Usuario? UsuariosMods { get; init; }

        public DateTime? DataMod { get; set; } = null;

        public bool IsAtivo { get; set; } = true;

        public IEnumerable<WardHashtag>? WardsHashtags { get; init; }
    }
}