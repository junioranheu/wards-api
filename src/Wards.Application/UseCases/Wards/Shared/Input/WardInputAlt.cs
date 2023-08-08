using Microsoft.AspNetCore.Http;

namespace Wards.Application.UseCases.Wards.Shared.Input
{
    public sealed class WardInputAlt
    {
        public int? WardId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Conteudo { get; set; } = string.Empty;

        // Propriedades extras;
        public int[]? ListaHashtags { get; set; } // Propriedade para reter os ids referentes às hashtags da ward em questão;

        public IFormFile? FormFileImagemPrincipal { get; set; }
    }
}