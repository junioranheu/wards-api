using System.Security.Claims;
using Wards.Domain.Entities;

namespace Wards.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GerarRefreshToken();
        string GerarToken(Usuario usuario, IEnumerable<Claim>? listaClaims);
        ClaimsPrincipal? GetInfoTokenExpirado(string? token);
    }
}