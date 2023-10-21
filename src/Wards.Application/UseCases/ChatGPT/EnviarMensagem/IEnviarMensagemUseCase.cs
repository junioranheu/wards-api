namespace Wards.Application.UseCases.ChatGPT.EnviarMensagem
{
    public interface IEnviarMensagemUseCase
    {
        Task<string> Execute(string input);
    }
}