using Wards.Application.UsesCases.Usuarios.Shared.Output;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuarioCondicaoArbitraria
{
    public interface IObterUsuarioCondicaoArbitrariaUseCase
    {
        Task<(UsuarioOutput? usuario, string senha)> Execute(string login);
    }
}