using AutoMapper;
using Wards.Application.UsesCases.Tokens.CriarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.CriarRefreshToken
{
    public sealed class CriarRefreshTokenUseCase : ICriarRefreshTokenUseCase
    {
        private readonly IMapper _map;
        private readonly ICriarRefreshTokenCommand _criarCommand;
        private readonly IDeletarRefreshTokenCommand _deletarCommand;

        public CriarRefreshTokenUseCase(IMapper map, ICriarRefreshTokenCommand criarCommand, IDeletarRefreshTokenCommand deletarCommand)
        {
            _map = map;
            _criarCommand = criarCommand;
            _deletarCommand = deletarCommand;
        }

        public async Task Execute(RefreshTokenInput input)
        {
            RefreshToken rt = _map.Map<RefreshToken>(input);

            await _deletarCommand.Execute(rt);
            await _criarCommand.Execute(rt);
        }
    }
}