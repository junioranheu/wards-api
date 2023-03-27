using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        await DBInitialize(app, isInitialize: true);

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
    AddHealthCheck(app);
    app.UseResponseCompression();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}

#region metodos_auxiliares
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

static void AddHealthCheck(WebApplication app)
{
    app.UseHealthChecks("/status", new HealthCheckOptions()
    {
        ResponseWriter = (httpContext, result) =>
        {
            httpContext.Response.ContentType = "application/json";

            #region objeto_json
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(p => new JProperty(p.Key, p.Value))))))))));
            #endregion

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    });
}
#endregion