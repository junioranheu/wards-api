using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UsesCases.Usuarios.AtualizarUsuario;
using Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.CriarUsuario;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.ListarUsuario;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UsesCases.Usuarios.ObterUsuario;
using Wards.Application.UsesCases.Usuarios.ObterUsuario.Queries;

namespace Wards.Application.UsesCases.Usuarios
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsuariosApplication(this IServiceCollection services)
        {
            services.AddScoped<IAtualizarUsuarioUseCase, AtualizarUsuarioUseCase>();
            services.AddScoped<IAtualizarUsuarioCommand, AtualizarUsuarioCommand>();

            services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
            services.AddScoped<ICriarUsuarioCommand, CriarUsuarioCommand>();

            services.AddScoped<IListarUsuarioUseCase, ListarUsuarioUseCase>();
            services.AddScoped<IListarUsuarioQuery, ListarUsuarioQuery>();

            services.AddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
            services.AddScoped<IObterUsuarioQuery, ObterUsuarioQuery>();

            return services;
        }
    }
}
