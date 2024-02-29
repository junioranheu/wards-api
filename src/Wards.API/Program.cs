using Wards.API;
using Wards.Application;
using Wards.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI();
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
    // builder.Services.AddDependencyInjectionWorkersServices(builder); // ***
}

WebApplication app = builder.Build();
{
    await app.UseAppConfigurationAsync(builder);
    app.Run();
}