using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Wards.Domain.DTOs;
using Wards.Infrastructure.Auth.Models;
using static Wards.Utils.Common;

namespace Wards.Infrastructure.Auth.Token
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public string GerarToken(UsuarioDTO usuario, IEnumerable<Claim>? listaClaims)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            SigningCredentials signingCredentials = new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            // Existem dois cenários possíveis para esse caso:
            // #2 - Cenário do Refresh Token, onde o Claim deve ser criado com base na lista de claims extraídas (método RefreshToken()), parâmetro listaClaims;
            // #1 - Cenário "normal", onde é feito o login ou cadastro e o Claim deve ser criado a partir do parâmetro UsuarioSenhaDTO;
            ClaimsIdentity claims;
            if (listaClaims?.Count() > 0)
            {
                claims = new ClaimsIdentity(listaClaims);
            }
            else
            {
                claims = new(new Claim[]
                {
                    new Claim(type: ClaimTypes.Name, usuario.NomeCompleto ?? string.Empty),
                    new Claim(type: ClaimTypes.Email, usuario.Email ?? string.Empty),

                    // Imitando o cenário do Azure, onde só tem e-mail e nome, e não role e ID;
                    // new Claim(type: ClaimTypes.Role, usuario.UsuarioTipoId.ToString()),
                    // new Claim(type: ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString())
                });
            }

            DateTime horarioBrasiliaAjustado = HorarioBrasiliaAjustado();
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = _jwtSettings.Issuer, // Emissor do token;
                IssuedAt = horarioBrasiliaAjustado,
                Audience = _jwtSettings.Audience,
                NotBefore = horarioBrasiliaAjustado,
                Expires = horarioBrasiliaAjustado.AddMinutes(_jwtSettings.TokenExpiryMinutes),
                Subject = claims,
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);
            return jwt;
        }

        // Como gerar um refresh token: https://www.youtube.com/watch?v=HsypCNm56zs&t=1021s&ab_channel=balta.io;
        public string GerarRefreshToken()
        {
            var numeroAleatorio = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(numeroAleatorio);
            var refreshToken = Convert.ToBase64String(numeroAleatorio);

            return refreshToken;
        }

        public ClaimsPrincipal? GetInfoTokenExpirado(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? "")),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal? principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
        }

        private static DateTime HorarioBrasiliaAjustado()
        {
            // Por algum motivo inexplicável é necessário ajustar a hora por uma diferença apresentada quando publicado em produção no Azure;
            // Produção: +3 horas;
            DateTime horarioBrasiliaAjustado = HorarioBrasilia().AddHours(+3);

#if DEBUG
            // Dev: horário normal;
            horarioBrasiliaAjustado = HorarioBrasilia();
#endif

            return horarioBrasiliaAjustado;
        }
    }
}
