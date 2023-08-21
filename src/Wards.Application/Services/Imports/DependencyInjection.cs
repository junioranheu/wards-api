﻿using Microsoft.Extensions.DependencyInjection;
using Wards.Application.Services.Imports.CSV;

namespace Wards.Application.Services.Imports
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddImportsService(this IServiceCollection services)
        {
            services.AddScoped<IImportCSVService, ImportCSVService>();

            return services;
        }
    }
}