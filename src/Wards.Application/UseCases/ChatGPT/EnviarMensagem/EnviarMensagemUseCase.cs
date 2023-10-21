using Wards.Application.UseCases.ChatGPT.EnviarMensagem.Commands;

namespace Wards.Application.UseCases.ChatGPT.EnviarMensagem
{
    public sealed class EnviarMensagemUseCase : IEnviarMensagemUseCase
    {
        private readonly IEnviarMensagemCommand _command;

        public EnviarMensagemUseCase(IEnviarMensagemCommand command)
        {
            _command = command;
        }

        public async Task<string> Execute(string input)
        {
            return await _command.Execute(input);
        }
    }
}