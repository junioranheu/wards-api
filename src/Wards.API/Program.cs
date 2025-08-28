using Wards.API;
using Wards.Application;
using Wards.Domain.Consts;
using Wards.Infrastructure;

Console.Title = SistemaConst.NomeSistema;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI(builder);
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
    // builder.Services.AddDependencyInjectionWorkersServices(builder); // ***
}

WebApplication app = builder.Build();
{
    await app.UseAppConfigurationAsync(builder);
    app.Run();
}