using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using Wards.Application.UseCases.ChatGPT.Shared.Output;

namespace Wards.Application.UseCases.ChatGPT.EnviarMensagem.Commands
{
    public class EnviarMensagemCommand : IEnviarMensagemCommand
    {
        private readonly IConfiguration _configuration;

        public EnviarMensagemCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Execute(string input)
        {
            CompletionCreateResponse respChatGPT = await ObterCompletionCreateResponse(input);
            ChatGPTResponse chatGPTResponse = ObterChatGPTResponse(respChatGPT);

            if (!chatGPTResponse.Successful)
            {
                return chatGPTResponse?.Error?.Message ?? string.Empty;
            }

            return chatGPTResponse?.Choices![0].Text ?? string.Empty;
        }

        private async Task<CompletionCreateResponse> ObterCompletionCreateResponse(string? texto)
        {
            OpenAIService gpt3 = new(new OpenAiOptions()
            {
                ApiKey = _configuration["ChatGPT:Key"] ?? string.Empty
            });

            CompletionCreateResponse respChatGPT = await gpt3.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = texto ?? string.Empty,
                Model = OpenAI.ObjectModels.Models.TextDavinciV2,
                Temperature = 0.5F,
                MaxTokens = 100,
                N = 1
            });

            return respChatGPT;
        }

        private static ChatGPTResponse ObterChatGPTResponse(CompletionCreateResponse respChatGPT)
        {
            ChatGPTResponse response = new()
            {
                Choices = respChatGPT.Choices,
                CreatedAt = respChatGPT.CreatedAt,
                Error = respChatGPT.Error,
                Id = respChatGPT.Id,
                Model = respChatGPT.Model,
                Successful = respChatGPT.Successful
            };

            return response;
        }
    }
}