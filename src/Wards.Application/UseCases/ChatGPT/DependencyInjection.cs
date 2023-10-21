using Microsoft.Extensions.DependencyInjection;
using Wards.Application.UseCases.ChatGPT.EnviarMensagem;
using Wards.Application.UseCases.ChatGPT.EnviarMensagem.Commands;

namespace Wards.Application.UseCases.ChatGPT
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddChatGPTApplication(this IServiceCollection services)
        {
            services.AddScoped<IEnviarMensagemUseCase, EnviarMensagemUseCase>();
            services.AddScoped<IEnviarMensagemCommand, EnviarMensagemCommand>();

            return services;
        }
    }
}