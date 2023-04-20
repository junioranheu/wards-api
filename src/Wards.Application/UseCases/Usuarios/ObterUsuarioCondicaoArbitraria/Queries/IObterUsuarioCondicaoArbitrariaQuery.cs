using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Usuarios.ObterUsuarioCondicaoArbitraria.Queries
{
    public interface IObterUsuarioCondicaoArbitrariaQuery
    {
        Task<(Usuario? usuario, string senha)> Execute(string login);
    }
}