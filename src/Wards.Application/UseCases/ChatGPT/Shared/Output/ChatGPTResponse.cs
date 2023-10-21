using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.SharedModels;

namespace Wards.Application.UseCases.ChatGPT.Shared.Output
{
    public sealed class ChatGPTResponse
    {
        public List<ChoiceResponse>? Choices { get; set; }

        public int CreatedAt { get; set; }

        public Error? Error { get; set; }

        public string Id { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public bool Successful { get; set; } = false;
    }
}