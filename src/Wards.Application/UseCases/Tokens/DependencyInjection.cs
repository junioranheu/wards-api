using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Tokens.CriarRefreshToken;
using Wards.Application.UseCases.Tokens.CriarRefreshToken.Commands;
using Wards.Application.UseCases.Tokens.DeletarRefreshToken;
using Wards.Application.UseCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Application.UseCases.Tokens.ObterRefreshToken;
using Wards.Application.UseCases.Tokens.ObterRefreshToken.Queries;

namespace Wards.Application.UseCases.Tokens
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTokensApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarRefreshTokenUseCase, CriarRefreshTokenUseCase>();
            services.AddScoped<ICriarRefreshTokenCommand, CriarRefreshTokenCommand>();

            services.AddScoped<IDeletarRefreshTokenUseCase, DeletarRefreshTokenUseCase>();
            services.AddScoped<IDeletarRefreshTokenCommand, DeletarRefreshTokenCommand>();

            services.AddScoped<IObterRefreshTokenUseCase, ObterRefreshTokenUseCase>();
            services.AddScoped<IObterRefreshTokenQuery, ObterRefreshTokenQuery>();

            return services;
        }
    }
}
