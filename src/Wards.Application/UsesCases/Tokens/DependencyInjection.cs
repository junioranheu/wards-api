using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.DeletarRefreshToken;
using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken;
using Wards.Application.UsesCases.Tokens.ObterRefreshToken.Queries;

namespace Wards.Application.UsesCases.Tokens
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTokensApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarRefreshTokenUsecase, CriarRefreshTokenUsecase>();
            services.AddScoped<ICriarRefreshTokenCommand, CriarRefreshTokenCommand>();

            services.AddScoped<IDeletarRefreshTokenUseCase, DeletarRefreshTokenUseCase>();
            services.AddScoped<IDeletarRefreshTokenCommand, DeletarRefreshTokenCommand>();

            services.AddScoped<IObterRefreshTokenUseCase, ObterRefreshTokenUseCase>();
            services.AddScoped<IObterRefreshTokenQuery, ObterRefreshTokenQuery>();

            return services;
        }
    }
}
