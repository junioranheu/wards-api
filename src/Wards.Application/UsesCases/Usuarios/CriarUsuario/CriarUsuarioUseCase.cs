using AutoMapper;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarUsuarioCommand _criarCommand;

        public CriarUsuarioUseCase(IMapper map, ICriarUsuarioCommand criarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
        }

        public async Task<UsuarioOutput> Execute(UsuarioInput input)
        {
            Usuario usuario = await _criarCommand.Execute(_map.Map<Usuario>(input));
            return _map.Map<UsuarioOutput>(usuario);
        }
    }
}