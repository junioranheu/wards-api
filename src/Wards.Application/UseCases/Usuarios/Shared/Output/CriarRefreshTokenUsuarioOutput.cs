using Wards.Application.UseCases.Shared.Models;

namespace Wards.Application.UseCases.Auths.Shared.Output
{
    public sealed class CriarRefreshTokenUsuarioOutput : ApiOutput
    {
        public string? Token { get; set; } = null;

        public string? RefreshToken { get; set; } = null;
    }
}