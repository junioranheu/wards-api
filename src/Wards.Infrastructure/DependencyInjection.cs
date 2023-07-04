﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Data;
using System.Text;
using Wards.Domain.Consts;
using Wards.Infrastructure.Auth.Models;
using Wards.Infrastructure.Auth.Token;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Factory;

namespace Wards.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddServices(services, builder);
            AddAuth(services, builder);
            AddFactory(services);
            AddContext(services, builder);
            AddSwagger(services);
            AddCors(services, builder);

            return services;
        }

        private static void AddServices(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
        }

        private static void AddAuth(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                 .AddJwtBearer(x =>
                 {
                     x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                     x.SaveToken = true;
                     x.IncludeErrorDetails = true;
                     x.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? string.Empty)),
                         ValidateIssuer = true,
                         ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                         ValidateAudience = true,
                         ValidAudience = builder.Configuration["JwtSettings:Audience"],
                         ValidateLifetime = true,
                         ClockSkew = TimeSpan.Zero
                     };
                 });
        }

        //private static void AddAuthAzure(IServiceCollection services, WebApplicationBuilder builder)
        //{
        //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //            .AddJwtBearer(x =>
        //            {
        //                x.Audience = builder.Configuration["AzureSettings:CliendId"] ?? string.Empty;
        //                x.Authority = builder.Configuration["AzureSettings:Authority"] ?? string.Empty;
        //                x.TokenValidationParameters = new TokenValidationParameters
        //                {
        //                    ValidateIssuer = false
        //                };
        //            });
        //}

        private static void AddFactory(IServiceCollection services)
        {
            services.AddSingleton<IConnectionFactory, ConnectionFactory>();
        }

        private static void AddContext(IServiceCollection services, WebApplicationBuilder builder)
        {
            string con = new ConnectionFactory(builder.Configuration).ObterStringConnection();

            // Entity Framework;
            services.AddDbContext<WardsContext>(x =>
            {
                // x.UseSqlServer(con);
                x.UseMySql(con, ServerVersion.AutoDetect(con));

#if DEBUG
                x.EnableSensitiveDataLogging();
#endif
            });

            // Dapper;
            services.AddScoped<IDbConnection>((sp) => new MySqlConnection(con));
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = $"{SistemaConst.NomeSistema}.API", Version = "v1" });

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

        private static void AddCors(IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddCors(x =>
                x.AddPolicy(name: builder.Configuration["CORSSettings:Cors"] ?? string.Empty, builder =>
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
