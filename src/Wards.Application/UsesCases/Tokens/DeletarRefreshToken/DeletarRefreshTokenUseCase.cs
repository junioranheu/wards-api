﻿using Wards.Application.UsesCases.Tokens.DeletarRefreshToken.Commands;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Tokens.DeletarRefreshToken
{
    public sealed class DeletarRefreshTokenUseCase : IDeletarRefreshTokenUseCase
    {
        public readonly IDeletarRefreshTokenCommand _deletarCommand;

        public DeletarRefreshTokenUseCase(IDeletarRefreshTokenCommand deletarCommand)
        {
            _deletarCommand = deletarCommand;
        }

        public async Task<bool> Deletar(RefreshToken input)
        {
            return await _deletarCommand.Deletar(input);
        }
    }
}
