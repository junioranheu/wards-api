using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewsletterCadastros.Application.UseCases.NewslettersCadastros;
using Wards.Application.AutoMapper;
using Wards.Application.Services.Exports;
using Wards.Application.Services.Imports;
using Wards.Application.Services.Sistemas;
using Wards.Application.Services.Usuarios;
using Wards.Application.UseCases.Auxiliares;
using Wards.Application.UseCases.ChatGPT;
using Wards.Application.UseCases.Feriados;
using Wards.Application.UseCases.FeriadosDatas;
using Wards.Application.UseCases.FeriadosEstados;
using Wards.Application.UseCases.Hashtags;
using Wards.Application.UseCases.Logs;
using Wards.Application.UseCases.Tokens;
using Wards.Application.UseCases.Usuarios;
using Wards.Application.UseCases.UsuariosRoles;
using Wards.Application.UseCases.Wards;
using Wards.Application.UseCases.WardsHashtags;

namespace Wards.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddAutoMapper(services);
            AddLogger(builder);

            AddUseCases(services);
            AddServices(services);

            return services;
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void AddLogger(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddLogsApplication();
            services.AddTokensApplication();
            services.AddUsuariosApplication();
            services.AddUsuariosRolesApplication();
            services.AddWardsApplication();
            services.AddHashtagsApplication();
            services.AddWardsHashtagsApplication();
            services.AddAuxiliaresApplication();
            services.AddFeriadosApplication();
            services.AddFeriadosDatasApplication();
            services.AddFeriadosEstadosApplication();
            services.AddNewslettersCadastrosApplication();
            services.AddChatGPTApplication();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddExportsService();
            services.AddImportsService();
            services.AddUsuariosService();
            services.AddResetarBancoDadosService();
        }
    }
}