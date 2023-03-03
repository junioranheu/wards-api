using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Wards.DesignPatterns.Adapter.AutoMapper;

namespace Wards.DesignPatterns.Adapter
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionDesignPatternsAdapter(this IServiceCollection services)
        {
            AddAutoMapper(services);

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
