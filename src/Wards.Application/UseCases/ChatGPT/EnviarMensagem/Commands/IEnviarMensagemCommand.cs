namespace Wards.Application.UseCases.ChatGPT.EnviarMensagem.Commands
{
    public interface IEnviarMensagemCommand
    {
        Task<string> Execute(string input);
    }
}