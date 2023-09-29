using Microsoft.Extensions.DependencyInjection;
using Wards.Infrastructure.UnitOfWork.Generic;

namespace Wards.Infrastructure.UnitOfWork
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnityOfWorkService(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}