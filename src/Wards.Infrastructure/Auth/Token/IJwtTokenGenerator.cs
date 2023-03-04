using System.Security.Claims;

namespace Wards.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GerarRefreshToken();
        string GerarToken(string nomeCompleto, string email, IEnumerable<Claim>? listaClaims);
        ClaimsPrincipal? GetInfoTokenExpirado(string? token);
    }
}