using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries;

namespace Wards.Application.UsesCases.Usuarios
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsuariosApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
            services.AddScoped<ICriarUsuarioCommand, CriarUsuarioCommand>();

            services.AddScoped<IListarUsuarioUseCase, ListarUsuarioUseCase>();
            services.AddScoped<IListarUsuarioQuery, ListarUsuarioQuery>();

            services.AddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
            services.AddScoped<IObterUsuarioQuery, ObterUsuarioQuery>();

            services.AddScoped<IObterUsuarioCondicaoArbitrariaUseCase, ObterUsuarioCondicaoArbitrariaUseCase>();
            services.AddScoped<IObterUsuarioCondicaoArbitrariaQuery, ObterUsuarioCondicaoArbitrariaQuery>();

            return services;
        }
    }
}