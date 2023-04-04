using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData;
using Wards.Application.UseCases.FeriadosDatas.CriarFeriadoData.Commands;
using Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData;
using Wards.Application.UseCases.FeriadosDatas.DeletarFeriadoData.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Wards.Application.UseCases.FeriadosDatas
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFeriadosDatasApplication(this IServiceCollection services)
        {
            services.AddScoped<ICriarFeriadoDataUseCase, CriarFeriadoDataUseCase>();
            services.AddScoped<ICriarFeriadoDataCommand, CriarFeriadoDataCommand>();

            services.AddScoped<IDeletarFeriadoDataUseCase, DeletarFeriadoDataUseCase>();
            services.AddScoped<IDeletarFeriadoDataCommand, DeletarFeriadoDataCommand>();

            return services;
        }
    }
}