using System.Security.Claims;
using Wards.Domain.DTOs;

namespace Wards.Infrastructure.Auth.Token
{
    public interface IJwtTokenGenerator
    {
        string GerarRefreshToken();
        string GerarToken(UsuarioDTO usuario, IEnumerable<Claim>? listaClaims);
        ClaimsPrincipal? GetInfoTokenExpirado(string? token);
    }
}