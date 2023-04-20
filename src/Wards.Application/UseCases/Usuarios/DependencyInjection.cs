using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.Usuarios.AutenticarUsuario;
using Wards.Application.UseCases.Usuarios.CriarRefreshTokenUsuario;
using Wards.Application.UseCases.Usuarios.CriarUsuario;
using Wards.Application.UseCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UseCases.Usuarios.ListarUsuario;
using Wards.Application.UseCases.Usuarios.ListarUsuario.Queries;
using Wards.Application.UseCases.Usuarios.ObterUsuario;
using Wards.Application.UseCases.Usuarios.ObterUsuario.Queries;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria;
using Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries;
using Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario;
using Wards.Application.UseCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands;
using Wards.Application.UseCases.Usuarios.VerificarContaUsuario;
using Wards.Application.UseCases.Usuarios.VerificarContaUsuario.Commands;

namespace Wards.Application.UseCases.Usuarios
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsuariosApplication(this IServiceCollection services)
        {
            services.AddScoped<IAutenticarUsuarioUseCase, AutenticarUsuarioUseCase>();

            services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
            services.AddScoped<ICriarUsuarioCommand, CriarUsuarioCommand>();

            services.AddScoped<ICriarRefreshTokenUsuarioUseCase, CriarRefreshTokenUsuarioUseCase>();

            services.AddScoped<IListarUsuarioUseCase, ListarUsuarioUseCase>();
            services.AddScoped<IListarUsuarioQuery, ListarUsuarioQuery>();

            services.AddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
            services.AddScoped<IObterUsuarioQuery, ObterUsuarioQuery>();

            services.AddScoped<IObterUsuarioCondicaoArbitrariaUseCase, ObterUsuarioCondicaoArbitrariaUseCase>();
            services.AddScoped<IObterUsuarioCondicaoArbitrariaQuery, ObterUsuarioCondicaoArbitrariaQuery>();

            services.AddScoped<IVerificarContaUsuarioUseCase, VerificarContaUsuarioUseCase>();
            services.AddScoped<IVerificarContaUsuarioCommand, VerificarContaUsuarioCommand>();

            services.AddScoped<ISolicitarVerificacaoContaUsuarioUseCase, SolicitarVerificacaoContaUsuarioUseCase>();
            services.AddScoped<ISolicitarVerificacaoContaUsuarioCommand, SolicitarVerificacaoContaUsuarioCommand>();
        
            return services;
        }
    }
}