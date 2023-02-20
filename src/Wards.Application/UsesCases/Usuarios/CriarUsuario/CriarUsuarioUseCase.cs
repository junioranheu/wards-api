using AutoMapper;
using Wards.Application.UsesCases.Usuarios.CriarUsuario.Commands;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.CriarUsuario
{
    public sealed class CriarUsuarioUseCase : ICriarUsuarioUseCase
    {
        public readonly ICriarUsuarioCommand _criarCommand;
        private readonly IMapper _map;

        public CriarUsuarioUseCase(ICriarUsuarioCommand criarCommand, IMapper map)
        {
            _criarCommand = criarCommand;
            _map = map;
        }

        public async Task<UsuarioDTO> Criar(Usuario input)
        {
            Usuario u = await _criarCommand.Criar(input);
            return _map.Map<UsuarioDTO>(u);
        }
    }
}
