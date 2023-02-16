using Swashbuckle.AspNetCore.SwaggerUI;
using Wards.API;
using Wards.Application;
using Wards.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI(builder);
    builder.Services.AddDependencyInjectionApplication();
    builder.Services.AddDependencyInjectionInfrastructure(builder);

    builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
    });
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Wards.API");
            c.RoutePrefix = "";
            c.DocExpansion(DocExpansion.None);
        });

        app.UseDeveloperExceptionPage();
    }

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(builder.Configuration["CORSSettings:Cors"] ?? "");

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}