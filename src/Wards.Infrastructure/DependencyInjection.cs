using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Wards.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddServices(services);
            AddAuth(services, builder);
            AddContext(builder);
            AddSwagger(builder);
            AddCors(builder);

            return services;
        }

        private static void AddServices(IServiceCollection services)
        {
           
        }

        private static void AddAuth(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(o =>
                    {
                        o.Audience = builder.Configuration["Azure:CliendId"] ?? string.Empty;
                        o.Authority = builder.Configuration["Azure:Authority"] ?? string.Empty;
                        o.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false
                        };
                    });
        }

        private static void AddContext(WebApplicationBuilder builder)
        {
            string con = builder.Configuration.GetConnectionString("xxx") ?? string.Empty;
            // builder.Services.AddDbContext<CargaVerificadaContext>(options => options.UseSqlServer(con));
        }

        private static void AddSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Wards.API", Version = "v1" });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Coloque **_apenas_** o token (JWT Bearer) abaixo!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }

        private static void AddCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
                options.AddPolicy(name: builder.Configuration["CORSSettings:Cors"] ?? string.Empty, builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials();
                })
            );
        }
    }
}
