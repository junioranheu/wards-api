using System.Security.Claims;
using System.Threading.RateLimiting;
using Wards.Domain.Consts;

namespace Wards.API.Extensions
{
    public static class RateLimitingConfig
    {
        public static IServiceCollection AddUserRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(SistemaConst.PolicyRateLimiting, httpContext =>
                {
                    string userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "anon";
                    string path = httpContext.Request.Path.Value?.ToLower() ?? "";
                    string key = $"{userId}:{path}";

                    return RateLimitPartition.GetTokenBucketLimiter(key, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 2,
                            TokensPerPeriod = 2,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                // Status code padrão 429;
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}