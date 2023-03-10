using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Wards.Application.AutoMapper;
using Wards.Application.Services.Import.CSV;
using Wards.Application.Services.Usuarios;
using Wards.Application.UsesCases.Auths;
using Wards.Application.UsesCases.Imports;
using Wards.Application.UsesCases.Logs;
using Wards.Application.UsesCases.Tokens;
using Wards.Application.UsesCases.Usuarios;
using Wards.Application.UsesCases.UsuariosRoles;

namespace Wards.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services)
        {
            AddAutoMapper(services);

            // UseCases;
            services.AddAuthsApplication();
            services.AddImportsApplication();
            services.AddLogsApplication();
            services.AddTokensApplication();
            services.AddUsuariosApplication();
            services.AddUsuariosRolesApplication();

            // Services;
            services.AddCsvImportService();
            services.AddUsuariosService();

            return services;
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}