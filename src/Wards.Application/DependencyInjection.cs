using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wards.Application.AutoMapper;
using Wards.Application.Services.Exports;
using Wards.Application.Services.Exports.CSV;
using Wards.Application.Services.Imports;
using Wards.Application.Services.Sistemas;
using Wards.Application.Services.Usuarios;
using Wards.Application.UseCases.Auxiliares;
using Wards.Application.UseCases.Feriados;
using Wards.Application.UseCases.FeriadosDatas;
using Wards.Application.UseCases.FeriadosEstados;
using Wards.Application.UseCases.Imports;
using Wards.Application.UseCases.Logs;
using Wards.Application.UseCases.Tokens;
using Wards.Application.UseCases.Usuarios;
using Wards.Application.UseCases.UsuariosRoles;
using Wards.Application.UseCases.Wards;

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
            services.AddImportsApplication();
            services.AddLogsApplication();
            services.AddTokensApplication();
            services.AddUsuariosApplication();
            services.AddUsuariosRolesApplication();
            services.AddWardsApplication();
            services.AddAuxiliaresApplication();
            services.AddFeriadosApplication();
            services.AddFeriadosDatasApplication();
            services.AddFeriadosEstadosApplication();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddImportCsvService();
            services.AddExportXlsxService();
            services.AddExportCsvService();
            services.AddUsuariosService();
            services.AddResetarBancoDadosService();
        }
    }
}