using Swashbuckle.AspNetCore.SwaggerUI;
using Wards.API;
using Wards.Application;
using Wards.Infrastructure;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI(builder);
    builder.Services.AddDependencyInjectionApplication();
    builder.Services.AddDependencyInjectionInfrastructure(builder);
}

WebApplication app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        await DBInitialize(app, isInitialize: false);

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wards.API");
            // c.RoutePrefix = "";
            c.DocExpansion(DocExpansion.None);
        });

        app.UseDeveloperExceptionPage();
    }

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(builder.Configuration["CORSSettings:Cors"]!);
    app.UseHealthChecks("/status");
    app.UseResponseCompression();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

static async Task DBInitialize(WebApplication app, bool isInitialize)
{
    if (isInitialize)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        try
        {
            WardsContext context = services.GetRequiredService<WardsContext>();
            await DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            string erroBD = ex.Message.ToString();
        }
    }
}