using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wards.Application.Hubs.ChatHub;
using Wards.Application.Hubs.RequestFilterHub;
using Wards.Domain.Consts;
using Wards.Domain.Enums;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;
using static Wards.Utils.Fixtures.Get;

namespace Wards.API
{
    public static class DependencyAppConfiguration
    {
        public static async Task<WebApplication> UseAppConfigurationAsync(this WebApplication app, WebApplicationBuilder builder)
        {
            using IServiceScope scope = app.Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            await DBInitialize(app, services, isInitialize: false);
            AddSwagger(app);
            AddHttpsRedirection(app);
            AddCors(app, builder);
            AddCompression(app);
            AddAuth(app);
            AddMisc(app);
            AddMapHubSignalR(app);
            AddHealthCheck(app);
            AddStaticFiles(app);

            return app;
        }

        private static async Task DBInitialize(WebApplication app, IServiceProvider services, bool isInitialize)
        {
            if (!app.Environment.IsDevelopment())
            {
                return;
            }

            if (!isInitialize)
            {
                return;
            }

            try
            {
                WardsContext context = services.GetRequiredService<WardsContext>();
                await DbInitializer.Initialize(context, isAplicarMigrations: false, isAplicarSeed: true);
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "{detalhes}", ObterDescricaoEnum(CodigoErroEnum.ErroDBInitialize));
                throw new Exception($"{ObterDescricaoEnum(CodigoErroEnum.ErroDBInitialize)}: {ex.InnerException?.ToString() ?? ex.Message}");
            }
        }

        private static void AddSwagger(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SistemaConst.NomeSistema}.API");
                    // c.RoutePrefix = ""; // ***
                    c.DocExpansion(DocExpansion.None);
                });

                app.UseDeveloperExceptionPage();
            }
        }

        private static void AddHttpsRedirection(WebApplication app)
        {
            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }
        }

        private static void AddCors(WebApplication app, WebApplicationBuilder builder)
        {
            app.UseCors(builder.Configuration["CORSSettings:Cors"]!);
        }

        private static void AddCompression(WebApplication app)
        {
            /// <summary>
            /// O trecho "app.UseWhen" abaixo é necessário quando a API tem uma resposta IAsyncEnumerable/Yield;
            /// O "UseResponseCompression" conflita com esse tipo de requisição, portanto é obrigatória a verificação abaixo;
            /// Caso não existam requisições desse tipo na API, é apenas necessário o trecho "app.UseResponseCompression()";
            /// </summary>
            app.UseWhen(context => !IsStreamingRequest(context), x =>
            {
                x.UseResponseCompression();
            });

            static bool IsStreamingRequest(HttpContext context)
            {
                Endpoint? endpoint = context.GetEndpoint();

                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    ControllerActionDescriptor? acao = routeEndpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                    if (acao is not null)
                    {
                        Type? tipo = acao.MethodInfo.ReturnType;

                        if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                        {
                            return true;
                        }

                        return false;
                    }
                }

                return false;
            }
        }

        private static void AddAuth(WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

        private static void AddMisc(WebApplication app)
        {
            app.UseResponseCaching();
            app.UseRateLimiter();
        }

        private static void AddMapHubSignalR(WebApplication app)
        {
            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<RequestFilterHub>("/requestFilterHub");
        }

        private static void AddHealthCheck(WebApplication app)
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

        private static void AddStaticFiles(WebApplication app)
        {
            IWebHostEnvironment env = app.Environment;

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Uploads")),
                RequestPath = "/Uploads",

                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                    ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                }
            });
        }
    }
}
